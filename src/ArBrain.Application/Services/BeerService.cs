using ArBrain.Application.DTOs;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Interfaces.Services;

namespace ArBrain.Application.Services;

public class BeerService(IBeerRepository beerRepository) : IBeerService
{
    public async Task<IReadOnlyList<BeerDto>> GetActiveBeersAsync(
        CancellationToken cancellationToken = default)
    {
        var beers = await beerRepository.GetActiveBeersAsync(cancellationToken);

        return beers
            .Select(b => new BeerDto(
                b.Id,
                b.Name,
                b.Style,
                b.Abv,
                b.Price,
                b.StockQuantity,
                b.MinimumStock,
                b.IsLowStock,
                b.CreatedAt))
            .ToList();
    }
}
