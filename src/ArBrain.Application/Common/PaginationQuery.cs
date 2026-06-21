namespace ArBrain.Application.Common;

public static class PaginationQuery
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public static (int Page, int PageSize, int Skip) Normalize(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedSize = pageSize switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => pageSize,
        };

        return (normalizedPage, normalizedSize, (normalizedPage - 1) * normalizedSize);
    }
}
