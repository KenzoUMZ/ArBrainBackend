using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using ArBrain.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Data;

/// <summary>
/// Popula o banco com dados iniciais para demonstração do fluxo fermentativo.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Beers.AnyAsync())
        {
            return;
        }

        var ipa = new Beer
        {
            Id = Guid.NewGuid(),
            Name = "ArBrain IPA",
            Style = BeerStyle.IPA,
        };

        ipa.FermentationParameters = new BeerFermentationParameters
        {
            Id = Guid.NewGuid(),
            BeerId = ipa.Id,
            MinTemperature = 18m,
            MaxTemperature = 22m,
            MinPh = 4.2m,
            MaxPh = 4.6m,
            MinExtract = 10m,
            MaxExtract = 14m,
        };

        var lager = new Beer
        {
            Id = Guid.NewGuid(),
            Name = "Golden Lager",
            Style = BeerStyle.Lager,
        };

        lager.FermentationParameters = new BeerFermentationParameters
        {
            Id = Guid.NewGuid(),
            BeerId = lager.Id,
            MinTemperature = 8m,
            MaxTemperature = 12m,
            MinPh = 4.4m,
            MaxPh = 4.8m,
            MinExtract = 9m,
            MaxExtract = 12m,
        };

        var tank1 = new Tank
        {
            Id = Guid.NewGuid(),
            Name = "Tanque FV-01",
            CapacityLiters = 1000m,
        };

        var tank2 = new Tank
        {
            Id = Guid.NewGuid(),
            Name = "Tanque FV-02",
            CapacityLiters = 1500m,
        };

        await context.Beers.AddRangeAsync(ipa, lager);
        await context.Tanks.AddRangeAsync(tank1, tank2);
        await context.SaveChangesAsync();

        // Lote de exemplo IPA001 com evolução ao longo de dois dias.
        var batchRecords = new List<FermentationRecord>
        {
            CreateRecord(ipa, tank1, "IPA001", new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc), 10m, 5.2m, 12m, null),
            CreateRecord(ipa, tank1, "IPA001", new DateTime(2026, 6, 2, 8, 0, 0, DateTimeKind.Utc), 10.5m, 5.1m, 11.5m, "Leve queda de pH"),
            CreateRecord(lager, tank2, "LAG001", new DateTime(2026, 6, 3, 9, 0, 0, DateTimeKind.Utc), 10m, 4.5m, 10m, "Dentro do padrão"),
        };

        await context.FermentationRecords.AddRangeAsync(batchRecords);
        await context.SaveChangesAsync();
    }

    private static FermentationRecord CreateRecord(
        Beer beer,
        Tank tank,
        string batchNumber,
        DateTime registeredAt,
        decimal temperature,
        decimal ph,
        decimal extract,
        string? observations)
    {
        var compliance = FermentationComplianceEvaluator.Evaluate(
            temperature,
            ph,
            extract,
            beer.FermentationParameters!);

        return new FermentationRecord
        {
            Id = Guid.NewGuid(),
            RegisteredAt = registeredAt,
            BeerId = beer.Id,
            TankId = tank.Id,
            BatchNumber = batchNumber,
            Temperature = temperature,
            Ph = ph,
            Extract = extract,
            Observations = observations,
            ComplianceStatus = compliance,
            Beer = beer,
            Tank = tank,
        };
    }
}
