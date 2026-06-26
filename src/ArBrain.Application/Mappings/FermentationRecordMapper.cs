using ArBrain.Application.DTOs.FermentationRecords;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Mappings;

/// <summary>Projeção de entidade de apontamento para DTO da API.</summary>
public static class FermentationRecordMapper
{
    public static FermentationRecordDto ToDto(FermentationRecord record) =>
        new(
            record.Id,
            record.RegisteredAt,
            record.BeerId,
            record.Beer.Name,
            record.TankId,
            record.Tank.Name,
            record.BatchNumber,
            record.Temperature,
            record.Ph,
            record.Extract,
            record.Observations,
            record.ComplianceStatus,
            record.CreatedAt);
}
