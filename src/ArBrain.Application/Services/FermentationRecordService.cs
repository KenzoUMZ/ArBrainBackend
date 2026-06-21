using ArBrain.Application.Common;
using ArBrain.Application.DTOs.FermentationRecords;
using ArBrain.Application.DTOs.Common;
using ArBrain.Domain.Enums;
using ArBrain.Application.Exceptions;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Interfaces.Services;
using ArBrain.Application.Mappings;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Services;

namespace ArBrain.Application.Services;

public class FermentationRecordService(
    IFermentationRecordRepository fermentationRecordRepository,
    IBeerRepository beerRepository,
    ITankRepository tankRepository) : IFermentationRecordService
{
    public async Task<PagedResult<FermentationRecordDto>> GetAllAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        FermentationComplianceStatus? complianceStatus = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var (normalizedPage, normalizedSize, _) = PaginationQuery.Normalize(page, pageSize);
        var (records, totalItems) = await fermentationRecordRepository.GetAllAsync(
            search, sortBy, sortDir, complianceStatus, normalizedPage, normalizedSize, cancellationToken);

        return new PagedResult<FermentationRecordDto>(
            records.Select(FermentationRecordMapper.ToDto).ToList(),
            normalizedPage,
            normalizedSize,
            totalItems);
    }

    public async Task<FermentationRecordDto> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await fermentationRecordRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Apontamento '{id}' não encontrado.");

        return FermentationRecordMapper.ToDto(record);
    }

    public async Task<FermentationRecordDto> CreateAsync(
        CreateFermentationRecordDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateCreateDto(dto);

        var beer = await beerRepository.GetByIdWithParametersAsync(dto.BeerId, cancellationToken)
            ?? throw new NotFoundException($"Cerveja '{dto.BeerId}' não encontrada.");

        if (beer.FermentationParameters is null)
        {
            throw new BusinessRuleException(
                $"A cerveja '{beer.Name}' não possui parâmetros fermentativos cadastrados.");
        }

        var tank = await tankRepository.GetByIdAsync(dto.TankId, cancellationToken)
            ?? throw new NotFoundException($"Tanque '{dto.TankId}' não encontrado.");

        // Calcula conformidade no momento do registro para alimentar dashboard e histórico.
        var complianceStatus = FermentationComplianceEvaluator.Evaluate(
            dto.Temperature,
            dto.Ph,
            dto.Extract,
            beer.FermentationParameters);

        var record = new FermentationRecord
        {
            Id = Guid.NewGuid(),
            RegisteredAt = dto.RegisteredAt,
            BeerId = beer.Id,
            TankId = tank.Id,
            BatchNumber = dto.BatchNumber.Trim().ToUpperInvariant(),
            Temperature = dto.Temperature,
            Ph = dto.Ph,
            Extract = dto.Extract,
            Observations = string.IsNullOrWhiteSpace(dto.Observations) ? null : dto.Observations.Trim(),
            ComplianceStatus = complianceStatus,
            Beer = beer,
            Tank = tank,
        };

        await fermentationRecordRepository.AddAsync(record, cancellationToken);
        return FermentationRecordMapper.ToDto(record);
    }

    public async Task<IReadOnlyList<BatchSummaryDto>> GetBatchSummariesAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        CancellationToken cancellationToken = default)
    {
        var summaries = await fermentationRecordRepository.GetBatchSummariesAsync(search, sortBy, sortDir, cancellationToken);

        return summaries
            .Select(item => new BatchSummaryDto(item.BatchNumber, item.BeerName, item.Count))
            .ToList();
    }

    public async Task<BatchHistoryDto> GetBatchHistoryAsync(
        string batchNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(batchNumber))
        {
            throw new BusinessRuleException("O número do lote é obrigatório.");
        }

        var normalizedBatch = batchNumber.Trim().ToUpperInvariant();
        var records = await fermentationRecordRepository.GetByBatchNumberAsync(
            normalizedBatch,
            cancellationToken);

        if (records.Count == 0)
        {
            throw new NotFoundException($"Nenhum apontamento encontrado para o lote '{normalizedBatch}'.");
        }

        return new BatchHistoryDto(
            normalizedBatch,
            records[0].Beer.Name,
            records.Select(FermentationRecordMapper.ToDto).ToList());
    }

    private static void ValidateCreateDto(CreateFermentationRecordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BatchNumber))
        {
            throw new BusinessRuleException("O número do lote é obrigatório.");
        }

        if (dto.RegisteredAt == default)
        {
            throw new BusinessRuleException("A data e hora do registro são obrigatórias.");
        }
    }
}
