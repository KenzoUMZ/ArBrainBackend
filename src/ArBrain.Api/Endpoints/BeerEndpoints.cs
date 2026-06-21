using ArBrain.Application.Common;
using ArBrain.Application.DTOs.Beers;
using ArBrain.Application.Interfaces.Services;

namespace ArBrain.Api.Endpoints;

public static class BeerEndpoints
{
    public static RouteGroupBuilder MapBeerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/beers").WithTags("Beers");

        group.MapGet("/", async (
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize,
            IBeerService service,
            CancellationToken ct) =>
            Results.Ok(await service.GetAllAsync(search, sortBy, sortDir, page, pageSize, ct)))
            .WithName("GetBeers");

        group.MapGet("/{id:guid}", async (Guid id, IBeerService service, CancellationToken ct) =>
            Results.Ok(await service.GetByIdAsync(id, ct)))
            .WithName("GetBeerById");

        group.MapPost("/", async (CreateBeerDto dto, IBeerService service, CancellationToken ct) =>
        {
            var created = await service.CreateAsync(dto, ct);
            return Results.Created($"/api/beers/{created.Id}", created);
        })
        .WithName("CreateBeer");

        group.MapPut("/{id:guid}", async (Guid id, UpdateBeerDto dto, IBeerService service, CancellationToken ct) =>
            Results.Ok(await service.UpdateAsync(id, dto, ct)))
            .WithName("UpdateBeer");

        group.MapDelete("/{id:guid}", async (Guid id, IBeerService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        })
        .WithName("DeleteBeer");

        group.MapPut("/{id:guid}/parameters", async (
            Guid id,
            UpsertBeerParametersDto dto,
            IBeerService service,
            CancellationToken ct) =>
            Results.Ok(await service.UpsertParametersAsync(id, dto, ct)))
            .WithName("UpsertBeerParameters");

        group.MapGet("/{id:guid}/parameters", async (Guid id, IBeerService service, CancellationToken ct) =>
            Results.Ok(await service.GetParametersAsync(id, ct)))
            .WithName("GetBeerParameters");

        return group;
    }
}
