using ArBrain.Application.DTOs.Tanks;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Mappings;

public static class TankMapper
{
    public static TankDto ToDto(Tank tank) =>
        new(tank.Id, tank.Name, tank.CapacityLiters, tank.CreatedAt);
}
