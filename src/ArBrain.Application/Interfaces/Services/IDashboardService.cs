using ArBrain.Application.DTOs.Dashboard;

namespace ArBrain.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
