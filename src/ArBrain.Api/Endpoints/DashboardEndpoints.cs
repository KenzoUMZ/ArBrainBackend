using ArBrain.Application.Interfaces.Services;

namespace ArBrain.Api.Endpoints;

/// <summary>Indicadores agregados da tela inicial.</summary>
public static class DashboardEndpoints
{
    public static RouteGroupBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard");

        group.MapGet("/", async (IDashboardService service, CancellationToken ct) =>
            Results.Ok(await service.GetSummaryAsync(ct)))
            .WithName("GetDashboardSummary");

        return group;
    }
}
