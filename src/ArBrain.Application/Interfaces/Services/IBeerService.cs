using ArBrain.Application.DTOs.Beers;

namespace ArBrain.Application.Interfaces.Services;

public interface IBeerService
{
    Task<IReadOnlyList<BeerDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BeerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<BeerDto> CreateAsync(CreateBeerDto dto, CancellationToken cancellationToken = default);

    Task<BeerDto> UpdateAsync(Guid id, UpdateBeerDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<BeerFermentationParametersDto> GetParametersAsync(
        Guid beerId,
        CancellationToken cancellationToken = default);

    Task<BeerFermentationParametersDto> UpsertParametersAsync(
        Guid beerId,
        UpsertBeerParametersDto dto,
        CancellationToken cancellationToken = default);
}
