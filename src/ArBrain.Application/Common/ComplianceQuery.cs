using ArBrain.Domain.Enums;

namespace ArBrain.Application.Common;

public static class ComplianceQuery
{
    public static FermentationComplianceStatus? ParseStatus(string? value) =>
        string.IsNullOrWhiteSpace(value) ||
        !Enum.TryParse<FermentationComplianceStatus>(value, ignoreCase: true, out var parsed)
            ? null
            : parsed;
}
