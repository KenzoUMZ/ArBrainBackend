using ArBrain.Application.Common;
using ArBrain.Application.DTOs.Common;
using ArBrain.Application.DTOs.Tanks;

namespace ArBrain.Application.Interfaces.Services;

public interface ITankService
{
    Task<PagedResult<TankDto>> GetAllAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        bool deletedOnly = false,
        CancellationToken cancellationToken = default);

    Task<TankDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TankDto> CreateAsync(CreateTankDto dto, CancellationToken cancellationToken = default);

    Task<TankDto> UpdateAsync(Guid id, UpdateTankDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task RestoreAsync(Guid id, CancellationToken cancellationToken = default);
}
