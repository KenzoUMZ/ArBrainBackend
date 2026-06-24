using ArBrain.Domain.Enums;

namespace ArBrain.Application.DTOs.Beers;

public record BeerDto(
    Guid Id,
    string Name,
    BeerStyle Style,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DeletedAt,
    BeerFermentationParametersDto? Parameters);

public record CreateBeerDto(string Name, BeerStyle Style);

public record UpdateBeerDto(string Name, BeerStyle Style);

public record BeerFermentationParametersDto(
    decimal MinTemperature,
    decimal MaxTemperature,
    decimal MinPh,
    decimal MaxPh,
    decimal MinExtract,
    decimal MaxExtract);

public record UpsertBeerParametersDto(
    decimal MinTemperature,
    decimal MaxTemperature,
    decimal MinPh,
    decimal MaxPh,
    decimal MinExtract,
    decimal MaxExtract);
