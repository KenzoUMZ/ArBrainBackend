using ArBrain.Application.Common;

namespace ArBrain.Application.Tests.Common;

public class PaginationQueryTests
{
    [Theory]
    [InlineData(1, 10, 1, 10, 0)]
    [InlineData(3, 20, 3, 20, 40)]
    [InlineData(0, 10, 1, 10, 0)]
    [InlineData(-5, 10, 1, 10, 0)]
    [InlineData(2, 0, 2, PaginationQuery.DefaultPageSize, PaginationQuery.DefaultPageSize)]
    [InlineData(2, -1, 2, PaginationQuery.DefaultPageSize, PaginationQuery.DefaultPageSize)]
    [InlineData(1, 150, 1, PaginationQuery.MaxPageSize, 0)]
    public void Normalize_ReturnsExpectedValues(
        int page,
        int pageSize,
        int expectedPage,
        int expectedSize,
        int expectedSkip)
    {
        var (normalizedPage, normalizedSize, skip) = PaginationQuery.Normalize(page, pageSize);

        Assert.Equal(expectedPage, normalizedPage);
        Assert.Equal(expectedSize, normalizedSize);
        Assert.Equal(expectedSkip, skip);
    }
}
