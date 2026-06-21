namespace ArBrain.Infrastructure.Data;

internal static class SortQuery
{
    public static bool IsDescending(string? sortDir) =>
        string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

    public static string? NormalizeField(string? sortBy) =>
        string.IsNullOrWhiteSpace(sortBy) ? null : sortBy.Trim().ToLowerInvariant();
}
