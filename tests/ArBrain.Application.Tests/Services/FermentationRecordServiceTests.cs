using ArBrain.Application.DTOs.FermentationRecords;
using ArBrain.Application.Exceptions;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Services;
using ArBrain.Application.Tests.Helpers;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using Moq;

namespace ArBrain.Application.Tests.Services;

public class FermentationRecordServiceTests
{
    private readonly Mock<IFermentationRecordRepository> _recordRepository = new();
    private readonly Mock<IBeerRepository> _beerRepository = new();
    private readonly Mock<ITankRepository> _tankRepository = new();
    private readonly FermentationRecordService _service;

    public FermentationRecordServiceTests()
    {
        _service = new FermentationRecordService(
            _recordRepository.Object,
            _beerRepository.Object,
            _tankRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenBatchNumberIsMissing_ThrowsBusinessRuleException()
    {
        var dto = new CreateFermentationRecordDto(
            RegisteredAt: DateTime.UtcNow,
            BeerId: Guid.NewGuid(),
            TankId: Guid.NewGuid(),
            BatchNumber: "  ",
            Temperature: 20m,
            Ph: 4.3m,
            Extract: 12m,
            Observations: null);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(dto));

        Assert.Equal("O número do lote é obrigatório.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenBeerHasNoParameters_ThrowsBusinessRuleException()
    {
        var beer = TestEntities.CreateBeerWithParameters();
        beer.FermentationParameters = null;
        var tank = TestEntities.CreateTank();
        var dto = new CreateFermentationRecordDto(
            RegisteredAt: DateTime.UtcNow,
            BeerId: beer.Id,
            TankId: tank.Id,
            BatchNumber: "ipa001",
            Temperature: 20m,
            Ph: 4.3m,
            Extract: 12m,
            Observations: "  observação  ");

        _beerRepository
            .Setup(repository => repository.GetByIdWithParametersAsync(beer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _tankRepository
            .Setup(repository => repository.GetByIdAsync(tank.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tank);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(dto));

        Assert.Equal($"A cerveja '{beer.Name}' não possui parâmetros fermentativos cadastrados.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_NormalizesBatchAndCalculatesCompliance()
    {
        var beer = TestEntities.CreateBeerWithParameters();
        var tank = TestEntities.CreateTank();
        var dto = new CreateFermentationRecordDto(
            RegisteredAt: new DateTime(2026, 6, 20, 14, 0, 0, DateTimeKind.Utc),
            BeerId: beer.Id,
            TankId: tank.Id,
            BatchNumber: " ipa001 ",
            Temperature: 17.9m,
            Ph: 4.3m,
            Extract: 12m,
            Observations: "  observação  ");
        FermentationRecord? persistedRecord = null;

        _beerRepository
            .Setup(repository => repository.GetByIdWithParametersAsync(beer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _tankRepository
            .Setup(repository => repository.GetByIdAsync(tank.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tank);
        _recordRepository
            .Setup(repository => repository.AddAsync(It.IsAny<FermentationRecord>(), It.IsAny<CancellationToken>()))
            .Callback<FermentationRecord, CancellationToken>((record, _) => persistedRecord = record)
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(persistedRecord);
        Assert.Equal("IPA001", persistedRecord!.BatchNumber);
        Assert.Equal(FermentationComplianceStatus.OutOfStandard, persistedRecord.ComplianceStatus);
        Assert.Equal("observação", persistedRecord.Observations);
        Assert.Equal("IPA001", result.BatchNumber);
        Assert.Equal(FermentationComplianceStatus.OutOfStandard, result.ComplianceStatus);
    }

    [Fact]
    public async Task GetBatchHistoryAsync_WhenBatchIsEmpty_ThrowsBusinessRuleException()
    {
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.GetBatchHistoryAsync("  "));

        Assert.Equal("O número do lote é obrigatório.", exception.Message);
    }

    [Fact]
    public async Task GetBatchHistoryAsync_WhenNoRecordsFound_ThrowsNotFoundException()
    {
        _recordRepository
            .Setup(repository => repository.GetByBatchNumberAsync("IPA999", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetBatchHistoryAsync("ipa999"));

        Assert.Equal("Nenhum apontamento encontrado para o lote 'IPA999'.", exception.Message);
    }

    [Fact]
    public async Task GetBatchHistoryAsync_WhenRecordsExist_ReturnsHistory()
    {
        var beer = TestEntities.CreateBeerWithParameters();
        var tank = TestEntities.CreateTank();
        var records = new List<FermentationRecord>
        {
            TestEntities.CreateRecord(beer, tank, "IPA001"),
            TestEntities.CreateRecord(beer, tank, "IPA001"),
        };

        _recordRepository
            .Setup(repository => repository.GetByBatchNumberAsync("IPA001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(records);

        var history = await _service.GetBatchHistoryAsync("ipa001");

        Assert.Equal("IPA001", history.BatchNumber);
        Assert.Equal(beer.Name, history.BeerName);
        Assert.Equal(2, history.Records.Count);
    }
}
