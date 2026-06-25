using ArBrain.Application.Mappings;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;

namespace ArBrain.Application.Tests.Mappings;

public class BeerMapperTests
{
    [Fact]
    public void ToDto_MapsBeerWithoutParameters()
    {
        var createdAt = new DateTime(2026, 6, 20, 10, 0, 0, DateTimeKind.Utc);
        var beer = new Beer
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "ArBrain IPA",
            Style = BeerStyle.IPA,
            CreatedAt = createdAt,
            UpdatedAt = null,
            DeletedAt = null,
            FermentationParameters = null,
        };

        var dto = BeerMapper.ToDto(beer);

        Assert.Equal(beer.Id, dto.Id);
        Assert.Equal("ArBrain IPA", dto.Name);
        Assert.Equal(BeerStyle.IPA, dto.Style);
        Assert.Equal(createdAt, dto.CreatedAt);
        Assert.Equal(createdAt, dto.UpdatedAt);
        Assert.Null(dto.DeletedAt);
        Assert.Null(dto.Parameters);
    }

    [Fact]
    public void ToDto_MapsBeerWithParameters()
    {
        var beer = new Beer
        {
            Id = Guid.NewGuid(),
            Name = "Golden Lager",
            Style = BeerStyle.Lager,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            FermentationParameters = new BeerFermentationParameters
            {
                MinTemperature = 10m,
                MaxTemperature = 14m,
                MinPh = 4.2m,
                MaxPh = 4.5m,
                MinExtract = 11m,
                MaxExtract = 13m,
            },
        };

        var dto = BeerMapper.ToDto(beer);

        Assert.NotNull(dto.Parameters);
        Assert.Equal(10m, dto.Parameters.MinTemperature);
        Assert.Equal(14m, dto.Parameters.MaxTemperature);
        Assert.Equal(4.2m, dto.Parameters.MinPh);
        Assert.Equal(4.5m, dto.Parameters.MaxPh);
        Assert.Equal(11m, dto.Parameters.MinExtract);
        Assert.Equal(13m, dto.Parameters.MaxExtract);
    }
}
