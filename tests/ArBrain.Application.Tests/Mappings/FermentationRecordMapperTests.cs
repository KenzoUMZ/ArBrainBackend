using ArBrain.Application.Mappings;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;

namespace ArBrain.Application.Tests.Mappings;

public class FermentationRecordMapperTests
{
    [Fact]
    public void ToDto_MapsAllFields()
    {
        var registeredAt = new DateTime(2026, 6, 20, 14, 30, 0, DateTimeKind.Utc);
        var createdAt = new DateTime(2026, 6, 20, 14, 31, 0, DateTimeKind.Utc);
        var beerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var tankId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var record = new FermentationRecord
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            RegisteredAt = registeredAt,
            BeerId = beerId,
            TankId = tankId,
            BatchNumber = "IPA001",
            Temperature = 20m,
            Ph = 4.4m,
            Extract = 12m,
            Observations = "Medição normal",
            ComplianceStatus = FermentationComplianceStatus.WithinStandard,
            CreatedAt = createdAt,
            Beer = new Beer { Id = beerId, Name = "ArBrain IPA" },
            Tank = new Tank { Id = tankId, Name = "Tanque FV-01" },
        };

        var dto = FermentationRecordMapper.ToDto(record);

        Assert.Equal(record.Id, dto.Id);
        Assert.Equal(registeredAt, dto.RegisteredAt);
        Assert.Equal(beerId, dto.BeerId);
        Assert.Equal("ArBrain IPA", dto.BeerName);
        Assert.Equal(tankId, dto.TankId);
        Assert.Equal("Tanque FV-01", dto.TankName);
        Assert.Equal("IPA001", dto.BatchNumber);
        Assert.Equal(20m, dto.Temperature);
        Assert.Equal(4.4m, dto.Ph);
        Assert.Equal(12m, dto.Extract);
        Assert.Equal("Medição normal", dto.Observations);
        Assert.Equal(FermentationComplianceStatus.WithinStandard, dto.ComplianceStatus);
        Assert.Equal(createdAt, dto.CreatedAt);
    }
}
