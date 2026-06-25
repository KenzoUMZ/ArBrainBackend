using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using ArBrain.Domain.Services;

namespace ArBrain.Domain.Tests.Services;

public class FermentationComplianceEvaluatorTests
{
    private static BeerFermentationParameters DefaultParameters() =>
        new()
        {
            MinTemperature = 18m,
            MaxTemperature = 22m,
            MinPh = 4.0m,
            MaxPh = 4.6m,
            MinExtract = 10m,
            MaxExtract = 14m,
        };

    [Fact]
    public void Evaluate_WhenAllMetricsAreCentered_ReturnsWithinStandard()
    {
        var parameters = DefaultParameters();

        var status = FermentationComplianceEvaluator.Evaluate(
            temperature: 20m,
            ph: 4.3m,
            extract: 12m,
            parameters);

        Assert.Equal(FermentationComplianceStatus.WithinStandard, status);
    }

    [Fact]
    public void Evaluate_WhenMetricIsNearLowerLimit_ReturnsRequiresAttention()
    {
        var parameters = DefaultParameters();

        var status = FermentationComplianceEvaluator.Evaluate(
            temperature: 18.3m,
            ph: 4.3m,
            extract: 12m,
            parameters);

        Assert.Equal(FermentationComplianceStatus.RequiresAttention, status);
    }

    [Fact]
    public void Evaluate_WhenMetricIsNearUpperLimit_ReturnsRequiresAttention()
    {
        var parameters = DefaultParameters();

        var status = FermentationComplianceEvaluator.Evaluate(
            temperature: 20m,
            ph: 4.3m,
            extract: 13.7m,
            parameters);

        Assert.Equal(FermentationComplianceStatus.RequiresAttention, status);
    }

    [Theory]
    [InlineData(17.9, 4.3, 12)]
    [InlineData(22.1, 4.3, 12)]
    [InlineData(20, 3.9, 12)]
    [InlineData(20, 4.7, 12)]
    [InlineData(20, 4.3, 9.9)]
    [InlineData(20, 4.3, 14.1)]
    public void Evaluate_WhenAnyMetricIsOutOfRange_ReturnsOutOfStandard(
        decimal temperature,
        decimal ph,
        decimal extract)
    {
        var parameters = DefaultParameters();

        var status = FermentationComplianceEvaluator.Evaluate(
            temperature,
            ph,
            extract,
            parameters);

        Assert.Equal(FermentationComplianceStatus.OutOfStandard, status);
    }

    [Fact]
    public void Evaluate_WhenRangeIsZeroAndValueMatches_ReturnsWithinStandard()
    {
        var parameters = new BeerFermentationParameters
        {
            MinTemperature = 20m,
            MaxTemperature = 20m,
            MinPh = 4.3m,
            MaxPh = 4.6m,
            MinExtract = 12m,
            MaxExtract = 14m,
        };

        var status = FermentationComplianceEvaluator.Evaluate(
            temperature: 20m,
            ph: 4.45m,
            extract: 13m,
            parameters);

        Assert.Equal(FermentationComplianceStatus.WithinStandard, status);
    }

    [Fact]
    public void Evaluate_WhenRangeIsZeroAndValueDiffers_ReturnsOutOfStandard()
    {
        var parameters = new BeerFermentationParameters
        {
            MinTemperature = 20m,
            MaxTemperature = 20m,
            MinPh = 4.3m,
            MaxPh = 4.6m,
            MinExtract = 12m,
            MaxExtract = 14m,
        };

        var status = FermentationComplianceEvaluator.Evaluate(
            temperature: 21m,
            ph: 4.3m,
            extract: 12m,
            parameters);

        Assert.Equal(FermentationComplianceStatus.OutOfStandard, status);
    }
}
