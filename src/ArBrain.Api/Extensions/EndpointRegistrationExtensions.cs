using ArBrain.Api.Endpoints;

namespace ArBrain.Api.Extensions;

public static class EndpointRegistrationExtensions
{
    public static WebApplication MapArBrainEndpoints(this WebApplication app)
    {
        app.MapGet("/api/health", () => Results.Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
        }))
        .WithName("HealthCheck")
        .WithTags("Health");

        app.MapBeerEndpoints();
        app.MapTankEndpoints();
        app.MapFermentationRecordEndpoints();
        app.MapDashboardEndpoints();

        return app;
    }
}
