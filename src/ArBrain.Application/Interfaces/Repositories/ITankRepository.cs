using ArBrain.Application.Common;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Interfaces.Repositories;

public interface ITankRepository
{
    Task<(IReadOnlyList<Tank> Items, int TotalItems)> GetAllActiveAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        bool deletedOnly = false,
        CancellationToken cancellationToken = default);

    Task<Tank?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Tank?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Tank tank, CancellationToken cancellationToken = default);

    Task UpdateAsync(Tank tank, CancellationToken cancellationToken = default);
}
