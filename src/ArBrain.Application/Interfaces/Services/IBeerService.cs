using ArBrain.Application.DTOs;

namespace ArBrain.Application.Interfaces.Services;

public interface IBeerService
{
    Task<IReadOnlyList<BeerDto>> GetActiveBeersAsync(CancellationToken cancellationToken = default);
}
