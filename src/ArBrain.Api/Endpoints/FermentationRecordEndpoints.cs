using ArBrain.Application.DTOs.FermentationRecords;
using ArBrain.Application.Interfaces.Services;

namespace ArBrain.Api.Endpoints;

public static class FermentationRecordEndpoints
{
    public static RouteGroupBuilder MapFermentationRecordEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/fermentation-records").WithTags("FermentationRecords");

        group.MapGet("/", async (IFermentationRecordService service, CancellationToken ct) =>
            Results.Ok(await service.GetAllAsync(ct)))
            .WithName("GetFermentationRecords");

        // Rotas literais antes de /{id} para evitar conflito com "batches".
        group.MapGet("/batches", async (IFermentationRecordService service, CancellationToken ct) =>
            Results.Ok(await service.GetBatchSummariesAsync(ct)))
            .WithName("GetBatchSummaries");

        group.MapGet("/batches/{batchNumber}", async (
            string batchNumber,
            IFermentationRecordService service,
            CancellationToken ct) =>
            Results.Ok(await service.GetBatchHistoryAsync(batchNumber, ct)))
            .WithName("GetBatchHistory");

        group.MapGet("/{id:guid}", async (Guid id, IFermentationRecordService service, CancellationToken ct) =>
            Results.Ok(await service.GetByIdAsync(id, ct)))
            .WithName("GetFermentationRecordById");

        group.MapPost("/", async (CreateFermentationRecordDto dto, IFermentationRecordService service, CancellationToken ct) =>
        {
            var created = await service.CreateAsync(dto, ct);
            return Results.Created($"/api/fermentation-records/{created.Id}", created);
        })
        .WithName("CreateFermentationRecord");

        return group;
    }
}
