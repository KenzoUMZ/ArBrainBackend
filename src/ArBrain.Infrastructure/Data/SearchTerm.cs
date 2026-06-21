namespace ArBrain.Infrastructure.Data;

internal static class SearchTerm
{
    public static string? Normalize(string? search) =>
        string.IsNullOrWhiteSpace(search) ? null : search.Trim();
}
