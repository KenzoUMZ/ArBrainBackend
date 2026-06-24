using ArBrain.Application.Common;
using ArBrain.Application.DTOs.Tanks;
using ArBrain.Application.Interfaces.Services;

namespace ArBrain.Api.Endpoints;

public static class TankEndpoints
{
    public static RouteGroupBuilder MapTankEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tanks").WithTags("Tanks");

        group.MapGet("/", async (
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize,
            bool? deletedOnly,
            ITankService service,
            CancellationToken ct) =>
            Results.Ok(await service.GetAllAsync(search, sortBy, sortDir, page, pageSize, deletedOnly ?? false, ct)))
            .WithName("GetTanks");

        group.MapGet("/{id:guid}", async (Guid id, ITankService service, CancellationToken ct) =>
            Results.Ok(await service.GetByIdAsync(id, ct)))
            .WithName("GetTankById");

        group.MapPost("/", async (CreateTankDto dto, ITankService service, CancellationToken ct) =>
        {
            var created = await service.CreateAsync(dto, ct);
            return Results.Created($"/api/tanks/{created.Id}", created);
        })
        .WithName("CreateTank");

        group.MapPut("/{id:guid}", async (Guid id, UpdateTankDto dto, ITankService service, CancellationToken ct) =>
            Results.Ok(await service.UpdateAsync(id, dto, ct)))
            .WithName("UpdateTank");

        group.MapDelete("/{id:guid}", async (Guid id, ITankService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteTank");

        group.MapPost("/{id:guid}/restore", async (Guid id, ITankService service, CancellationToken ct) =>
        {
            await service.RestoreAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("RestoreTank");

        return group;
    }
}
