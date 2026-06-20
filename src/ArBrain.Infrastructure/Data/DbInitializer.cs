using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Beers.AnyAsync())
        {
            return;
        }

        var beers = new List<Beer>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "ArBrain IPA",
                Style = BeerStyle.IPA,
                Abv = 6.5m,
                Price = 18.90m,
                StockQuantity = 120,
                MinimumStock = 20,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Golden Lager",
                Style = BeerStyle.Lager,
                Abv = 4.8m,
                Price = 12.50m,
                StockQuantity = 200,
                MinimumStock = 30,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Midnight Stout",
                Style = BeerStyle.Stout,
                Abv = 7.2m,
                Price = 21.00m,
                StockQuantity = 8,
                MinimumStock = 15,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Citrus Pale Ale",
                Style = BeerStyle.PaleAle,
                Abv = 5.4m,
                Price = 16.75m,
                StockQuantity = 45,
                MinimumStock = 25,
            },
        };

        await context.Beers.AddRangeAsync(beers);
        await context.SaveChangesAsync();
    }
}
