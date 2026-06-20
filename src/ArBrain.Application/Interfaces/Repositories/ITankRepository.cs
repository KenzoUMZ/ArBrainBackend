using ArBrain.Domain.Entities;

namespace ArBrain.Application.Interfaces.Repositories;

public interface ITankRepository
{
    Task<IReadOnlyList<Tank>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    Task<Tank?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Tank tank, CancellationToken cancellationToken = default);

    Task UpdateAsync(Tank tank, CancellationToken cancellationToken = default);
}
