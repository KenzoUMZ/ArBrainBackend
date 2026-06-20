using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;

namespace ArBrain.Application.Interfaces.Repositories;

public interface IFermentationRecordRepository
{
    Task<IReadOnlyList<FermentationRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<FermentationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FermentationRecord>> GetByBatchNumberAsync(
        string batchNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<(string BatchNumber, string BeerName, int Count)>> GetBatchSummariesAsync(
        CancellationToken cancellationToken = default);

    Task<Dictionary<FermentationComplianceStatus, int>> GetComplianceCountsAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(FermentationRecord record, CancellationToken cancellationToken = default);
}
