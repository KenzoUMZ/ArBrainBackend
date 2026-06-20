using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Domain.Entities;
using ArBrain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Repositories;

public class BeerRepository(AppDbContext context) : IBeerRepository
{
    public async Task<IReadOnlyList<Beer>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Beers
            .AsNoTracking()
            .Include(b => b.FermentationParameters)
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Beer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Beers
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive, cancellationToken);
    }

    public async Task<Beer?> GetByIdWithParametersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Beers
            .Include(b => b.FermentationParameters)
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Beers.Where(b => b.IsActive && b.Name == name.Trim());

        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Beer beer, CancellationToken cancellationToken = default)
    {
        await context.Beers.AddAsync(beer, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Beer beer, CancellationToken cancellationToken = default)
    {
        var entry = context.Entry(beer);
        if (entry.State == EntityState.Detached)
        {
            context.Beers.Update(beer);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertFermentationParametersAsync(
        Guid beerId,
        BeerFermentationParameters parameters,
        bool isNew,
        CancellationToken cancellationToken = default)
    {
        if (isNew)
        {
            parameters.BeerId = beerId;
            await context.BeerFermentationParameters.AddAsync(parameters, cancellationToken);
        }

        var beer = await context.Beers.FindAsync([beerId], cancellationToken);
        if (beer is not null)
        {
            beer.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
