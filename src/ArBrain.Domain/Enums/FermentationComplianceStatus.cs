namespace ArBrain.Domain.Enums;

/// <summary>
/// Classificação de um apontamento fermentativo em relação aos parâmetros aceitáveis da cerveja.
/// </summary>
public enum FermentationComplianceStatus
{
    /// <summary>Todos os indicadores dentro da faixa aceitável.</summary>
    WithinStandard = 1,

    /// <summary>Indicadores dentro da faixa, porém próximos aos limites.</summary>
    RequiresAttention = 2,

    /// <summary>Pelo menos um indicador fora da faixa aceitável.</summary>
    OutOfStandard = 3,
}
