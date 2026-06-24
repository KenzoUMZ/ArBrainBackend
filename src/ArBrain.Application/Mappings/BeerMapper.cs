using ArBrain.Application.DTOs.Beers;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Mappings;

/// <summary>
/// Converte entidades de domínio em DTOs de resposta da API.
/// </summary>
public static class BeerMapper
{
    public static BeerDto ToDto(Beer beer) =>
        new(
            beer.Id,
            beer.Name,
            beer.Style,
            beer.CreatedAt,
            beer.UpdatedAt ?? beer.CreatedAt,
            beer.DeletedAt,
            beer.FermentationParameters is null
                ? null
                : ToParametersDto(beer.FermentationParameters));

    public static BeerFermentationParametersDto ToParametersDto(BeerFermentationParameters parameters) =>
        new(
            parameters.MinTemperature,
            parameters.MaxTemperature,
            parameters.MinPh,
            parameters.MaxPh,
            parameters.MinExtract,
            parameters.MaxExtract);
}
