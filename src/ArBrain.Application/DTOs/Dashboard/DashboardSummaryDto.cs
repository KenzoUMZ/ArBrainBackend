namespace ArBrain.Application.DTOs.Dashboard;

public record DashboardSummaryDto(
    int TotalRecords,
    int WithinStandardCount,
    int RequiresAttentionCount,
    int OutOfStandardCount);
