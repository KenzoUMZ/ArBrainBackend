using ArBrain.Domain.Enums;

namespace ArBrain.Application.DTOs.FermentationRecords;

public record FermentationRecordDto(
    Guid Id,
    DateTime RegisteredAt,
    Guid BeerId,
    string BeerName,
    Guid TankId,
    string TankName,
    string BatchNumber,
    decimal Temperature,
    decimal Ph,
    decimal Extract,
    string? Observations,
    FermentationComplianceStatus ComplianceStatus,
    DateTime CreatedAt);

public record CreateFermentationRecordDto(
    DateTime RegisteredAt,
    Guid BeerId,
    Guid TankId,
    string BatchNumber,
    decimal Temperature,
    decimal Ph,
    decimal Extract,
    string? Observations);

public record BatchSummaryDto(string BatchNumber, string BeerName, int RecordCount);

public record BatchHistoryDto(
    string BatchNumber,
    string BeerName,
    IReadOnlyList<FermentationRecordDto> Records);
