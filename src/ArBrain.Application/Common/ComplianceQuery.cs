using ArBrain.Domain.Enums;

namespace ArBrain.Application.Common;

/// <summary>Converte o filtro de conformidade recebido na query string da API.</summary>
public static class ComplianceQuery
{
    public static FermentationComplianceStatus? ParseStatus(string? value) =>
        string.IsNullOrWhiteSpace(value) ||
        !Enum.TryParse<FermentationComplianceStatus>(value, ignoreCase: true, out var parsed)
            ? null
            : parsed;
}
