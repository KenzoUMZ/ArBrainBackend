using ArBrain.Application.Common;
using ArBrain.Application.DTOs.Beers;
using ArBrain.Application.DTOs.Common;
using ArBrain.Application.Exceptions;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Interfaces.Services;
using ArBrain.Application.Mappings;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Services;

/// <summary>Cadastro de cervejas, parâmetros fermentativos e inativação lógica (soft delete).</summary>
public class BeerService(IBeerRepository beerRepository) : IBeerService
{
    public async Task<PagedResult<BeerDto>> GetAllAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        bool deletedOnly = false,
        CancellationToken cancellationToken = default)
    {
        var (normalizedPage, normalizedSize, _) = PaginationQuery.Normalize(page, pageSize);
        var (beers, totalItems) = await beerRepository.GetAllActiveAsync(
            search, sortBy, sortDir, normalizedPage, normalizedSize, deletedOnly, cancellationToken);

        return new PagedResult<BeerDto>(
            beers.Select(BeerMapper.ToDto).ToList(),
            normalizedPage,
            normalizedSize,
            totalItems);
    }

    public async Task<BeerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var beer = await beerRepository.GetByIdWithParametersAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Cerveja '{id}' não encontrada.");

        return BeerMapper.ToDto(beer);
    }

    public async Task<BeerDto> CreateAsync(CreateBeerDto dto, CancellationToken cancellationToken = default)
    {
        ValidateBeerName(dto.Name);

        if (await beerRepository.ExistsByNameAsync(dto.Name, cancellationToken: cancellationToken))
        {
            throw new BusinessRuleException($"Já existe uma cerveja com o nome '{dto.Name}'.");
        }

        var beer = new Beer
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Style = dto.Style,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await beerRepository.AddAsync(beer, cancellationToken);
        return BeerMapper.ToDto(beer);
    }

    public async Task<BeerDto> UpdateAsync(
        Guid id,
        UpdateBeerDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateBeerName(dto.Name);

        var beer = await beerRepository.GetByIdWithParametersAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Cerveja '{id}' não encontrada.");

        if (await beerRepository.ExistsByNameAsync(dto.Name, id, cancellationToken))
        {
            throw new BusinessRuleException($"Já existe uma cerveja com o nome '{dto.Name}'.");
        }

        beer.Name = dto.Name.Trim();
        beer.Style = dto.Style;
        beer.UpdatedAt = DateTime.UtcNow;

        await beerRepository.UpdateAsync(beer, cancellationToken);
        return BeerMapper.ToDto(beer);
    }

    /// <summary>Inativa a cerveja (soft delete) sem remover registros históricos.</summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var beer = await beerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Cerveja '{id}' não encontrada.");

        beer.IsActive = false;
        beer.UpdatedAt = DateTime.UtcNow;
        beer.DeletedAt = DateTime.UtcNow;
        await beerRepository.UpdateAsync(beer, cancellationToken);
    }

    public async Task RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var beer = await beerRepository.GetByIdIncludingDeletedAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Cerveja '{id}' não encontrada.");

        if (beer.IsActive)
        {
            throw new BusinessRuleException("Esta cerveja já está ativa.");
        }

        if (await beerRepository.ExistsByNameAsync(beer.Name, cancellationToken: cancellationToken))
        {
            throw new BusinessRuleException($"Já existe uma cerveja ativa com o nome '{beer.Name}'.");
        }

        beer.IsActive = true;
        beer.DeletedAt = null;
        beer.UpdatedAt = DateTime.UtcNow;
        await beerRepository.UpdateAsync(beer, cancellationToken);
    }

    public async Task<BeerFermentationParametersDto> GetParametersAsync(
        Guid beerId,
        CancellationToken cancellationToken = default)
    {
        var beer = await beerRepository.GetByIdWithParametersAsync(beerId, cancellationToken)
            ?? throw new NotFoundException($"Cerveja '{beerId}' não encontrada.");

        if (beer.FermentationParameters is null)
        {
            throw new NotFoundException($"A cerveja '{beer.Name}' ainda não possui parâmetros cadastrados.");
        }

        return BeerMapper.ToParametersDto(beer.FermentationParameters);
    }

    /// <summary>Cria ou atualiza os limites aceitáveis usados na classificação de conformidade.</summary>
    public async Task<BeerFermentationParametersDto> UpsertParametersAsync(
        Guid beerId,
        UpsertBeerParametersDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateParameters(dto);

        var beer = await beerRepository.GetByIdWithParametersAsync(beerId, cancellationToken)
            ?? throw new NotFoundException($"Cerveja '{beerId}' não encontrada.");

        var isNew = beer.FermentationParameters is null;
        BeerFermentationParameters parameters;

        if (isNew)
        {
            parameters = new BeerFermentationParameters
            {
                Id = Guid.NewGuid(),
                BeerId = beer.Id,
                MinTemperature = dto.MinTemperature,
                MaxTemperature = dto.MaxTemperature,
                MinPh = dto.MinPh,
                MaxPh = dto.MaxPh,
                MinExtract = dto.MinExtract,
                MaxExtract = dto.MaxExtract,
            };
        }
        else
        {
            parameters = beer.FermentationParameters!;
            parameters.MinTemperature = dto.MinTemperature;
            parameters.MaxTemperature = dto.MaxTemperature;
            parameters.MinPh = dto.MinPh;
            parameters.MaxPh = dto.MaxPh;
            parameters.MinExtract = dto.MinExtract;
            parameters.MaxExtract = dto.MaxExtract;
            parameters.UpdatedAt = DateTime.UtcNow;
        }

        await beerRepository.UpsertFermentationParametersAsync(
            beerId,
            parameters,
            isNew,
            cancellationToken);

        return BeerMapper.ToParametersDto(parameters);
    }

    private static void ValidateBeerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException("O nome da cerveja é obrigatório.");
        }
    }

    /// <summary>Garante que os valores mínimos sejam menores que os máximos.</summary>
    private static void ValidateParameters(UpsertBeerParametersDto dto)
    {
        if (dto.MinTemperature >= dto.MaxTemperature)
        {
            throw new BusinessRuleException("A temperatura mínima deve ser menor que a máxima.");
        }

        if (dto.MinPh >= dto.MaxPh)
        {
            throw new BusinessRuleException("O pH mínimo deve ser menor que o máximo.");
        }

        if (dto.MinExtract >= dto.MaxExtract)
        {
            throw new BusinessRuleException("O extrato mínimo deve ser menor que o máximo.");
        }
    }
}
