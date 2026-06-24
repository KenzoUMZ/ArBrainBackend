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
    private const int DevelopmentBeerCount = 20;
    private const int DevelopmentTankCount = 20;
    private const int DevelopmentBatchCount = 20;

    /// <summary>
    /// Migra o banco e popula com dados mínimos apenas se ainda estiver vazio.
    /// </summary>
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Beers.AnyAsync())
        {
            return;
        }

        await SeedMinimalAsync(context);
    }

    /// <summary>
    /// Apaga todos os dados existentes, migra e re-popula com um conjunto rico de dados
    /// pensado para testar paginação, filtros e ordenação em todas as telas.
    /// </summary>
    public static async Task ResetAndSeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE fermentation_records, beer_fermentation_parameters, tanks, beers RESTART IDENTITY CASCADE");

        await SeedDevelopmentAsync(context);
    }

    // -------------------------------------------------------------------------
    // Dados mínimos (2 cervejas, 2 tanques, 2 lotes, 3 apontamentos)
    // -------------------------------------------------------------------------

    private static async Task SeedMinimalAsync(AppDbContext context)
    {
        var ipa = CreateBeer("ArBrain IPA", BeerStyle.IPA, 18m, 22m, 4.2m, 4.6m, 10m, 14m);
        var lager = CreateBeer("Golden Lager", BeerStyle.Lager, 8m, 12m, 4.4m, 4.8m, 9m, 12m);

        var tank1 = CreateTank("Tanque FV-01", 1000m);
        var tank2 = CreateTank("Tanque FV-02", 1500m);

        await context.Beers.AddRangeAsync(ipa, lager);
        await context.Tanks.AddRangeAsync(tank1, tank2);
        await context.SaveChangesAsync();

        var batchRecords = new List<FermentationRecord>
        {
            Rec(ipa, tank1, "IPA001", Dt(2026, 6, 1, 8), 20.0m, 4.40m, 12.0m, null),
            Rec(ipa, tank1, "IPA001", Dt(2026, 6, 2, 8), 20.5m, 4.35m, 11.5m, "Leve queda de pH"),
            Rec(lager, tank2, "LAG001", Dt(2026, 6, 3, 9), 10.0m, 4.50m, 10.0m, "Dentro do padrão"),
        };

        await context.FermentationRecords.AddRangeAsync(batchRecords);
        await context.SaveChangesAsync();
    }

    // -------------------------------------------------------------------------
    // Dados de desenvolvimento — 20 cervejas, 20 tanques, 20 lotes
    // -------------------------------------------------------------------------

    private static async Task SeedDevelopmentAsync(AppDbContext context)
    {
        var beerTemplates = new (string Name, BeerStyle Style, decimal MinTemp, decimal MaxTemp, decimal MinPh, decimal MaxPh, decimal MinExtract, decimal MaxExtract)[]
        {
            ("ArBrain IPA", BeerStyle.IPA, 18m, 22m, 4.2m, 4.6m, 10m, 14m),
            ("Citrus IPA", BeerStyle.IPA, 18m, 22m, 4.2m, 4.6m, 10m, 14m),
            ("West Coast IPA", BeerStyle.IPA, 19m, 23m, 4.1m, 4.5m, 11m, 15m),
            ("Golden Lager", BeerStyle.Lager, 8m, 12m, 4.4m, 4.8m, 9m, 12m),
            ("Pilsen Clara", BeerStyle.Pilsner, 7m, 11m, 4.3m, 4.7m, 8m, 11m),
            ("Bohemian Pils", BeerStyle.Pilsner, 8m, 12m, 4.4m, 4.8m, 9m, 12m),
            ("Escura Stout", BeerStyle.Stout, 15m, 19m, 4.0m, 4.4m, 12m, 16m),
            ("Imperial Stout", BeerStyle.Stout, 16m, 20m, 4.0m, 4.3m, 14m, 18m),
            ("Chocolate Porter", BeerStyle.Porter, 16m, 20m, 4.1m, 4.5m, 13m, 17m),
            ("Robusta Porter", BeerStyle.Porter, 15m, 19m, 4.0m, 4.4m, 12m, 16m),
            ("Trigo Weiss", BeerStyle.Weiss, 16m, 20m, 4.3m, 4.7m, 11m, 15m),
            ("Bavarian Weiss", BeerStyle.Weiss, 17m, 21m, 4.2m, 4.6m, 10m, 14m),
            ("Azeda Sour", BeerStyle.Sour, 20m, 26m, 3.0m, 3.8m, 8m, 12m),
            ("Frutas Vermelhas Sour", BeerStyle.Sour, 21m, 27m, 3.1m, 3.9m, 7m, 11m),
            ("Pale Session Ale", BeerStyle.PaleAle, 17m, 21m, 4.2m, 4.6m, 9m, 13m),
            ("American Pale Ale", BeerStyle.PaleAle, 18m, 22m, 4.1m, 4.5m, 10m, 14m),
            ("Hazy Pale Ale", BeerStyle.PaleAle, 19m, 23m, 4.0m, 4.4m, 11m, 15m),
            ("Dry Hop Lager", BeerStyle.Lager, 9m, 13m, 4.3m, 4.7m, 8m, 11m),
            ("Black IPA", BeerStyle.IPA, 17m, 21m, 4.1m, 4.5m, 11m, 15m),
            ("Saison Farmhouse", BeerStyle.PaleAle, 20m, 24m, 3.8m, 4.2m, 8m, 12m),
        };

        var beers = beerTemplates
            .Take(DevelopmentBeerCount)
            .Select(t => CreateBeer(t.Name, t.Style, t.MinTemp, t.MaxTemp, t.MinPh, t.MaxPh, t.MinExtract, t.MaxExtract))
            .ToList();

        var tanks = Enumerable.Range(1, DevelopmentTankCount)
            .Select(i => CreateTank($"Tanque FV-{i:D2}", 800m + i * 100m))
            .ToList();

        await context.Beers.AddRangeAsync(beers);
        await context.Tanks.AddRangeAsync(tanks);
        await context.SaveChangesAsync();

        var batchPrefixes = new Dictionary<BeerStyle, string>
        {
            [BeerStyle.IPA] = "IPA",
            [BeerStyle.Lager] = "LAG",
            [BeerStyle.Pilsner] = "PIL",
            [BeerStyle.Stout] = "STO",
            [BeerStyle.Porter] = "POR",
            [BeerStyle.Weiss] = "WHE",
            [BeerStyle.Sour] = "SOU",
            [BeerStyle.PaleAle] = "PAL",
        };

        var styleCounters = new Dictionary<BeerStyle, int>();
        var records = new List<FermentationRecord>();
        var startDate = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);

        for (var batchIndex = 0; batchIndex < DevelopmentBatchCount; batchIndex++)
        {
            var beer = beers[batchIndex];
            var tank = tanks[batchIndex % tanks.Count];
            var prefix = batchPrefixes[beer.Style];
            styleCounters.TryGetValue(beer.Style, out var counter);
            counter++;
            styleCounters[beer.Style] = counter;

            var batchNumber = $"{prefix}{counter:D3}";
            var batchStart = startDate.AddDays(batchIndex * 3);
            var recordCount = 6 + batchIndex % 3;

            for (var day = 0; day < recordCount; day++)
            {
                var hour = day % 2 == 0 ? 8 : 20;
                var registeredAt = batchStart.AddDays(day).AddHours(hour - 8);
                var progress = (decimal)day / Math.Max(recordCount - 1, 1);

                var temperature = Lerp(beer.FermentationParameters!.MinTemperature, beer.FermentationParameters.MaxTemperature, 0.35m + progress * 0.3m);
                var ph = Lerp(beer.FermentationParameters.MinPh, beer.FermentationParameters.MaxPh, 0.4m + progress * 0.2m);
                var extract = Lerp(beer.FermentationParameters.MaxExtract, beer.FermentationParameters.MinExtract, progress * 0.55m);

                if (batchIndex % 5 == 4 && day == recordCount - 2)
                {
                    temperature = beer.FermentationParameters.MaxTemperature + 1.2m;
                }
                else if (batchIndex % 7 == 3 && day == recordCount - 1)
                {
                    ph = beer.FermentationParameters.MinPh - 0.08m;
                }
                else if (batchIndex % 6 == 2 && day == recordCount - 3)
                {
                    temperature = beer.FermentationParameters.MaxTemperature - 0.15m;
                    ph = beer.FermentationParameters.MinPh + 0.05m;
                }

                string? observations = null;
                if (day == recordCount - 1)
                {
                    observations = "Último apontamento do lote";
                }
                else if (batchIndex % 4 == 0 && day == 2)
                {
                    observations = "Medição com leve desvio";
                }

                records.Add(Rec(beer, tank, batchNumber, registeredAt, temperature, ph, extract, observations));
            }
        }

        await context.FermentationRecords.AddRangeAsync(records);
        await context.SaveChangesAsync();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Beer CreateBeer(
        string name,
        BeerStyle style,
        decimal minTemp,
        decimal maxTemp,
        decimal minPh,
        decimal maxPh,
        decimal minExtract,
        decimal maxExtract)
    {
        var now = DateTime.UtcNow;
        var beer = new Beer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Style = style,
            CreatedAt = now,
            UpdatedAt = now,
        };

        beer.FermentationParameters = new BeerFermentationParameters
        {
            Id = Guid.NewGuid(),
            BeerId = beer.Id,
            MinTemperature = minTemp,
            MaxTemperature = maxTemp,
            MinPh = minPh,
            MaxPh = maxPh,
            MinExtract = minExtract,
            MaxExtract = maxExtract,
        };

        return beer;
    }

    private static Tank CreateTank(string name, decimal capacityLiters)
    {
        var now = DateTime.UtcNow;
        return new Tank
        {
            Id = Guid.NewGuid(),
            Name = name,
            CapacityLiters = capacityLiters,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    private static decimal Lerp(decimal from, decimal to, decimal t) =>
        from + (to - from) * t;

    private static DateTime Dt(int year, int month, int day, int hour) =>
        new(year, month, day, hour, 0, 0, DateTimeKind.Utc);

    private static FermentationRecord Rec(
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
            temperature, ph, extract, beer.FermentationParameters!);

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
