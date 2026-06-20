using ArBrain.Application.DTOs.FermentationRecords;

namespace ArBrain.Application.Interfaces.Services;

public interface IFermentationRecordService
{
    Task<IReadOnlyList<FermentationRecordDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<FermentationRecordDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FermentationRecordDto> CreateAsync(
        CreateFermentationRecordDto dto,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BatchSummaryDto>> GetBatchSummariesAsync(
        CancellationToken cancellationToken = default);

    Task<BatchHistoryDto> GetBatchHistoryAsync(
        string batchNumber,
        CancellationToken cancellationToken = default);
}
