using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Domain.Entities;
using ArBrain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Repositories;

public class BeerRepository(AppDbContext context) : IBeerRepository
{
    public async Task<IReadOnlyList<Beer>> GetActiveBeersAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Beers
            .AsNoTracking()
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Beer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Beers
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive, cancellationToken);
    }
}
