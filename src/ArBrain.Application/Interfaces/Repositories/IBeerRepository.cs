using ArBrain.Domain.Entities;

namespace ArBrain.Application.Interfaces.Repositories;

public interface IBeerRepository
{
    Task<IReadOnlyList<Beer>> GetActiveBeersAsync(CancellationToken cancellationToken = default);

    Task<Beer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
