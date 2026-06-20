namespace ArBrain.Application.DTOs.Tanks;

public record TankDto(
    Guid Id,
    string Name,
    decimal CapacityLiters,
    DateTime CreatedAt);

public record CreateTankDto(string Name, decimal CapacityLiters);

public record UpdateTankDto(string Name, decimal CapacityLiters);
