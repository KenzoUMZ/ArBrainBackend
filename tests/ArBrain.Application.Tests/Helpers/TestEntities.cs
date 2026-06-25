using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;

namespace ArBrain.Application.Tests.Helpers;

internal static class TestEntities
{
    public static Beer CreateBeerWithParameters(
        string name = "ArBrain IPA",
        BeerStyle style = BeerStyle.IPA)
    {
        var beer = new Beer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Style = style,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        beer.FermentationParameters = new BeerFermentationParameters
        {
            Id = Guid.NewGuid(),
            BeerId = beer.Id,
            MinTemperature = 18m,
            MaxTemperature = 22m,
            MinPh = 4.0m,
            MaxPh = 4.6m,
            MinExtract = 10m,
            MaxExtract = 14m,
        };

        return beer;
    }

    public static Tank CreateTank(string name = "Tanque FV-01", decimal capacityLiters = 1000m) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            CapacityLiters = capacityLiters,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
        };

    public static FermentationRecord CreateRecord(
        Beer beer,
        Tank tank,
        string batchNumber = "IPA001")
    {
        return new FermentationRecord
        {
            Id = Guid.NewGuid(),
            RegisteredAt = new DateTime(2026, 6, 20, 14, 0, 0, DateTimeKind.Utc),
            BeerId = beer.Id,
            TankId = tank.Id,
            BatchNumber = batchNumber,
            Temperature = 20m,
            Ph = 4.3m,
            Extract = 12m,
            ComplianceStatus = FermentationComplianceStatus.WithinStandard,
            CreatedAt = DateTime.UtcNow,
            Beer = beer,
            Tank = tank,
        };
    }
}
