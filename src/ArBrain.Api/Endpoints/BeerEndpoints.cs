using ArBrain.Application.Interfaces.Services;

namespace ArBrain.Api.Endpoints;

public static class BeerEndpoints
{
    public static RouteGroupBuilder MapBeerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/beers")
            .WithTags("Beers");

        group.MapGet("/", async (
            IBeerService beerService,
            CancellationToken cancellationToken) =>
        {
            var beers = await beerService.GetActiveBeersAsync(cancellationToken);
            return Results.Ok(beers);
        })
        .WithName("GetBeers");

        return group;
    }
}
