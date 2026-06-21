using ArBrain.Application.Common;
using ArBrain.Application.DTOs.Common;
using ArBrain.Application.DTOs.FermentationRecords;
using ArBrain.Domain.Enums;

namespace ArBrain.Application.Interfaces.Services;

public interface IFermentationRecordService
{
    Task<PagedResult<FermentationRecordDto>> GetAllAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        FermentationComplianceStatus? complianceStatus = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        CancellationToken cancellationToken = default);

    Task<FermentationRecordDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FermentationRecordDto> CreateAsync(
        CreateFermentationRecordDto dto,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BatchSummaryDto>> GetBatchSummariesAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        CancellationToken cancellationToken = default);

    Task<BatchHistoryDto> GetBatchHistoryAsync(
        string batchNumber,
        CancellationToken cancellationToken = default);
}
