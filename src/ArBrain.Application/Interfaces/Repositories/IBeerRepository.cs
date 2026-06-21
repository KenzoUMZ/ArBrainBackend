using ArBrain.Application.Common;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Interfaces.Repositories;

public interface IBeerRepository
{
    Task<(IReadOnlyList<Beer> Items, int TotalItems)> GetAllActiveAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<Beer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Beer?> GetByIdWithParametersAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Beer beer, CancellationToken cancellationToken = default);

    Task UpdateAsync(Beer beer, CancellationToken cancellationToken = default);

    Task UpsertFermentationParametersAsync(
        Guid beerId,
        BeerFermentationParameters parameters,
        bool isNew,
        CancellationToken cancellationToken = default);
}
