using ArBrain.Application.DTOs.Tanks;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Mappings;

/// <summary>Projeção de entidade de tanque para DTO da API.</summary>
public static class TankMapper
{
    public static TankDto ToDto(Tank tank) =>
        new(tank.Id, tank.Name, tank.CapacityLiters, tank.CreatedAt, tank.UpdatedAt ?? tank.CreatedAt, tank.DeletedAt);
}
