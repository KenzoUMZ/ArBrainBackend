using ArBrain.Application.DTOs.Beers;
using ArBrain.Application.Exceptions;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Services;
using ArBrain.Application.Tests.Helpers;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using Moq;

namespace ArBrain.Application.Tests.Services;

public class BeerServiceTests
{
    private readonly Mock<IBeerRepository> _repository = new();
    private readonly BeerService _service;

    public BeerServiceTests()
    {
        _service = new BeerService(_repository.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsEmpty_ThrowsBusinessRuleException()
    {
        var dto = new CreateBeerDto("   ", BeerStyle.IPA);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(dto));

        Assert.Equal("O nome da cerveja é obrigatório.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenNameAlreadyExists_ThrowsBusinessRuleException()
    {
        var dto = new CreateBeerDto("ArBrain IPA", BeerStyle.IPA);
        _repository
            .Setup(repository => repository.ExistsByNameAsync(dto.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(dto));

        Assert.Equal("Já existe uma cerveja com o nome 'ArBrain IPA'.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_PersistsAndReturnsDto()
    {
        var dto = new CreateBeerDto("  ArBrain IPA  ", BeerStyle.IPA);
        Beer? persistedBeer = null;

        _repository
            .Setup(repository => repository.ExistsByNameAsync(dto.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repository
            .Setup(repository => repository.AddAsync(It.IsAny<Beer>(), It.IsAny<CancellationToken>()))
            .Callback<Beer, CancellationToken>((beer, _) => persistedBeer = beer)
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(persistedBeer);
        Assert.Equal("ArBrain IPA", persistedBeer!.Name);
        Assert.Equal(BeerStyle.IPA, persistedBeer.Style);
        Assert.Equal("ArBrain IPA", result.Name);
        _repository.Verify(repository => repository.AddAsync(It.IsAny<Beer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBeerDoesNotExist_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository
            .Setup(repository => repository.GetByIdWithParametersAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(id));
    }

    [Fact]
    public async Task UpsertParametersAsync_WhenMinIsGreaterThanMax_ThrowsBusinessRuleException()
    {
        var beer = TestEntities.CreateBeerWithParameters();
        var dto = new UpsertBeerParametersDto(22m, 18m, 4.0m, 4.6m, 10m, 14m);

        _repository
            .Setup(repository => repository.GetByIdWithParametersAsync(beer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.UpsertParametersAsync(beer.Id, dto));

        Assert.Equal("A temperatura mínima deve ser menor que a máxima.", exception.Message);
    }

    [Fact]
    public async Task RestoreAsync_WhenBeerIsAlreadyActive_ThrowsBusinessRuleException()
    {
        var beer = TestEntities.CreateBeerWithParameters();
        beer.IsActive = true;

        _repository
            .Setup(repository => repository.GetByIdIncludingDeletedAsync(beer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.RestoreAsync(beer.Id));

        Assert.Equal("Esta cerveja já está ativa.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WhenBeerExists_SoftDeletes()
    {
        var beer = TestEntities.CreateBeerWithParameters();

        _repository
            .Setup(repository => repository.GetByIdAsync(beer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);

        await _service.DeleteAsync(beer.Id);

        Assert.False(beer.IsActive);
        Assert.NotNull(beer.DeletedAt);
        _repository.Verify(repository => repository.UpdateAsync(beer, It.IsAny<CancellationToken>()), Times.Once);
    }
}
