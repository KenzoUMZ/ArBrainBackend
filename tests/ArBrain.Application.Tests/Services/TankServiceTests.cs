using ArBrain.Application.DTOs.Tanks;
using ArBrain.Application.Exceptions;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Services;
using ArBrain.Application.Tests.Helpers;
using ArBrain.Domain.Entities;
using Moq;

namespace ArBrain.Application.Tests.Services;

public class TankServiceTests
{
    private readonly Mock<ITankRepository> _repository = new();
    private readonly TankService _service;

    public TankServiceTests()
    {
        _service = new TankService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCapacityIsZero_ThrowsBusinessRuleException()
    {
        var dto = new CreateTankDto("Tanque FV-01", 0m);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(dto));

        Assert.Equal("A capacidade do tanque deve ser maior que zero.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenNameAlreadyExists_ThrowsBusinessRuleException()
    {
        var dto = new CreateTankDto("Tanque FV-01", 1000m);
        _repository
            .Setup(repository => repository.ExistsByNameAsync(dto.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(dto));

        Assert.Equal("Já existe um tanque com o nome 'Tanque FV-01'.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_PersistsAndReturnsDto()
    {
        var dto = new CreateTankDto("  Tanque FV-01  ", 1500m);
        Tank? persistedTank = null;

        _repository
            .Setup(repository => repository.ExistsByNameAsync(dto.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repository
            .Setup(repository => repository.AddAsync(It.IsAny<Tank>(), It.IsAny<CancellationToken>()))
            .Callback<Tank, CancellationToken>((tank, _) => persistedTank = tank)
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(persistedTank);
        Assert.Equal("Tanque FV-01", persistedTank!.Name);
        Assert.Equal(1500m, persistedTank.CapacityLiters);
        Assert.Equal("Tanque FV-01", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTankDoesNotExist_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository
            .Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tank?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(id));
    }

    [Fact]
    public async Task RestoreAsync_WhenActiveNameConflictExists_ThrowsBusinessRuleException()
    {
        var tank = TestEntities.CreateTank();
        tank.IsActive = false;
        tank.DeletedAt = DateTime.UtcNow;

        _repository
            .Setup(repository => repository.GetByIdIncludingDeletedAsync(tank.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tank);
        _repository
            .Setup(repository => repository.ExistsByNameAsync(tank.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.RestoreAsync(tank.Id));

        Assert.Equal($"Já existe um tanque ativo com o nome '{tank.Name}'.", exception.Message);
    }
}
