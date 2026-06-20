using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Domain.Entities;
using ArBrain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Repositories;

public class TankRepository(AppDbContext context) : ITankRepository
{
    public async Task<IReadOnlyList<Tank>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Tanks
            .AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tank?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Tanks
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Tanks.Where(t => t.IsActive && t.Name == name.Trim());

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Tank tank, CancellationToken cancellationToken = default)
    {
        await context.Tanks.AddAsync(tank, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Tank tank, CancellationToken cancellationToken = default)
    {
        context.Tanks.Update(tank);
        await context.SaveChangesAsync(cancellationToken);
    }
}
