using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;

namespace ArBrain.Domain.Services;

/// <summary>
/// Avalia se um apontamento fermentativo está dentro, próximo ou fora dos parâmetros aceitáveis.
/// </summary>
public static class FermentationComplianceEvaluator
{
    /// <summary>
    /// Percentual da faixa aceitável usado como margem de alerta (10% nas extremidades).
    /// </summary>
    private const decimal AttentionMarginRatio = 0.10m;

    /// <summary>
    /// Classifica o apontamento com base nos três indicadores.
    /// Prioridade: fora do padrão &gt; requer atenção (10% das extremidades) &gt; dentro do padrão.
    /// </summary>
    public static FermentationComplianceStatus Evaluate(
        decimal temperature,
        decimal ph,
        decimal extract,
        BeerFermentationParameters parameters)
    {
        var temperatureStatus = EvaluateMetric(
            temperature,
            parameters.MinTemperature,
            parameters.MaxTemperature);

        var phStatus = EvaluateMetric(
            ph,
            parameters.MinPh,
            parameters.MaxPh);

        var extractStatus = EvaluateMetric(
            extract,
            parameters.MinExtract,
            parameters.MaxExtract);

        // Qualquer indicador fora da faixa classifica o registro como fora do padrão.
        if (temperatureStatus == MetricStatus.OutOfRange ||
            phStatus == MetricStatus.OutOfRange ||
            extractStatus == MetricStatus.OutOfRange)
        {
            return FermentationComplianceStatus.OutOfStandard;
        }

        // Dentro da faixa, porém próximo aos limites, exige atenção operacional.
        if (temperatureStatus == MetricStatus.NearLimit ||
            phStatus == MetricStatus.NearLimit ||
            extractStatus == MetricStatus.NearLimit)
        {
            return FermentationComplianceStatus.RequiresAttention;
        }

        return FermentationComplianceStatus.WithinStandard;
    }

    private static MetricStatus EvaluateMetric(decimal value, decimal min, decimal max)
    {
        if (value < min || value > max)
        {
            return MetricStatus.OutOfRange;
        }

        var range = max - min;
        if (range <= 0)
        {
            return value == min ? MetricStatus.WithinRange : MetricStatus.OutOfRange;
        }

        var margin = range * AttentionMarginRatio;
        var nearMin = value <= min + margin;
        var nearMax = value >= max - margin;

        return nearMin || nearMax ? MetricStatus.NearLimit : MetricStatus.WithinRange;
    }

    private enum MetricStatus
    {
        WithinRange,
        NearLimit,
        OutOfRange,
    }
}
