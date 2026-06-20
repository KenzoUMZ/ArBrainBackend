using ArBrain.Application.DTOs.Tanks;
using ArBrain.Application.Exceptions;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Interfaces.Services;
using ArBrain.Application.Mappings;
using ArBrain.Domain.Entities;

namespace ArBrain.Application.Services;

public class TankService(ITankRepository tankRepository) : ITankService
{
    public async Task<IReadOnlyList<TankDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tanks = await tankRepository.GetAllActiveAsync(cancellationToken);
        return tanks.Select(TankMapper.ToDto).ToList();
    }

    public async Task<TankDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tank = await tankRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tanque '{id}' não encontrado.");

        return TankMapper.ToDto(tank);
    }

    public async Task<TankDto> CreateAsync(CreateTankDto dto, CancellationToken cancellationToken = default)
    {
        ValidateTank(dto.Name, dto.CapacityLiters);

        if (await tankRepository.ExistsByNameAsync(dto.Name, cancellationToken: cancellationToken))
        {
            throw new BusinessRuleException($"Já existe um tanque com o nome '{dto.Name}'.");
        }

        var tank = new Tank
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            CapacityLiters = dto.CapacityLiters,
        };

        await tankRepository.AddAsync(tank, cancellationToken);
        return TankMapper.ToDto(tank);
    }

    public async Task<TankDto> UpdateAsync(
        Guid id,
        UpdateTankDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidateTank(dto.Name, dto.CapacityLiters);

        var tank = await tankRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tanque '{id}' não encontrado.");

        if (await tankRepository.ExistsByNameAsync(dto.Name, id, cancellationToken))
        {
            throw new BusinessRuleException($"Já existe um tanque com o nome '{dto.Name}'.");
        }

        tank.Name = dto.Name.Trim();
        tank.CapacityLiters = dto.CapacityLiters;
        tank.UpdatedAt = DateTime.UtcNow;

        await tankRepository.UpdateAsync(tank, cancellationToken);
        return TankMapper.ToDto(tank);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tank = await tankRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tanque '{id}' não encontrado.");

        tank.IsActive = false;
        tank.UpdatedAt = DateTime.UtcNow;
        await tankRepository.UpdateAsync(tank, cancellationToken);
    }

    private static void ValidateTank(string name, decimal capacityLiters)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException("O nome do tanque é obrigatório.");
        }

        if (capacityLiters <= 0)
        {
            throw new BusinessRuleException("A capacidade do tanque deve ser maior que zero.");
        }
    }
}
