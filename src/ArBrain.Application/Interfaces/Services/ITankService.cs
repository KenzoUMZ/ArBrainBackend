using ArBrain.Application.DTOs.Tanks;

namespace ArBrain.Application.Interfaces.Services;

public interface ITankService
{
    Task<IReadOnlyList<TankDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TankDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TankDto> CreateAsync(CreateTankDto dto, CancellationToken cancellationToken = default);

    Task<TankDto> UpdateAsync(Guid id, UpdateTankDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
