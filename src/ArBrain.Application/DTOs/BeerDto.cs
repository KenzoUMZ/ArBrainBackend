using ArBrain.Domain.Enums;

namespace ArBrain.Application.DTOs;

public record BeerDto(
    Guid Id,
    string Name,
    BeerStyle Style,
    decimal Abv,
    decimal Price,
    int StockQuantity,
    int MinimumStock,
    bool IsLowStock,
    DateTime CreatedAt);
