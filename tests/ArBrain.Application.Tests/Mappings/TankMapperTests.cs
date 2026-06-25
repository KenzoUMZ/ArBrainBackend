using ArBrain.Application.Mappings;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Tests.Mappings;

public class TankMapperTests
{
    [Fact]
    public void ToDto_MapsAllFields()
    {
        var createdAt = new DateTime(2026, 6, 20, 12, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2026, 6, 21, 8, 0, 0, DateTimeKind.Utc);
        var deletedAt = new DateTime(2026, 6, 24, 15, 0, 0, DateTimeKind.Utc);

        var tank = new Tank
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Tanque FV-01",
            CapacityLiters = 1000m,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
        };

        var dto = TankMapper.ToDto(tank);

        Assert.Equal(tank.Id, dto.Id);
        Assert.Equal("Tanque FV-01", dto.Name);
        Assert.Equal(1000m, dto.CapacityLiters);
        Assert.Equal(createdAt, dto.CreatedAt);
        Assert.Equal(updatedAt, dto.UpdatedAt);
        Assert.Equal(deletedAt, dto.DeletedAt);
    }
}
