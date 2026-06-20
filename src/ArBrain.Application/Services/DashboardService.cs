using ArBrain.Application.DTOs.Dashboard;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Interfaces.Services;
using ArBrain.Domain.Enums;

namespace ArBrain.Application.Services;

public class DashboardService(IFermentationRecordRepository fermentationRecordRepository)
    : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var counts = await fermentationRecordRepository.GetComplianceCountsAsync(cancellationToken);

        var withinStandard = counts.GetValueOrDefault(FermentationComplianceStatus.WithinStandard, 0);
        var requiresAttention = counts.GetValueOrDefault(FermentationComplianceStatus.RequiresAttention, 0);
        var outOfStandard = counts.GetValueOrDefault(FermentationComplianceStatus.OutOfStandard, 0);

        return new DashboardSummaryDto(
            withinStandard + requiresAttention + outOfStandard,
            withinStandard,
            requiresAttention,
            outOfStandard);
    }
}
