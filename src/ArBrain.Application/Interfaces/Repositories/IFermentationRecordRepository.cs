using ArBrain.Application.Common;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;

namespace ArBrain.Application.Interfaces.Repositories;

public interface IFermentationRecordRepository
{
    Task<(IReadOnlyList<FermentationRecord> Items, int TotalItems)> GetAllAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        FermentationComplianceStatus? complianceStatus = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<FermentationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FermentationRecord>> GetByBatchNumberAsync(
        string batchNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(string BatchNumber, string BeerName, int Count)>> GetBatchSummariesAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        CancellationToken cancellationToken = default);

    Task<Dictionary<FermentationComplianceStatus, int>> GetComplianceCountsAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(FermentationRecord record, CancellationToken cancellationToken = default);
}
