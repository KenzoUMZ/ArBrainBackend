using ArBrain.Application.Common;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Domain.Entities;
using ArBrain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Repositories;

public class BeerRepository(AppDbContext context) : IBeerRepository
{
    public async Task<(IReadOnlyList<Beer> Items, int TotalItems)> GetAllActiveAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var term = SearchTerm.Normalize(search)?.ToLowerInvariant();
        var sortField = SortQuery.NormalizeField(sortBy);
        var descending = SortQuery.IsDescending(sortDir);
        var (_, normalizedSize, skip) = PaginationQuery.Normalize(page, pageSize);

        var query = context.Beers
            .AsNoTracking()
            .Include(b => b.FermentationParameters)
            .Where(b => b.IsActive);

        if (term is not null)
        {
            query = query.Where(b =>
                b.Name.ToLower().Contains(term) ||
                b.Style.ToString().ToLower().Contains(term));
        }

        query = (sortField, descending) switch
        {
            ("style", false) => query.OrderBy(b => b.Style).ThenBy(b => b.Name),
            ("style", true) => query.OrderByDescending(b => b.Style).ThenBy(b => b.Name),
            ("parameters", false) => query.OrderBy(b => b.FermentationParameters == null).ThenBy(b => b.Name),
            ("parameters", true) => query.OrderByDescending(b => b.FermentationParameters == null).ThenBy(b => b.Name),
            ("name", true) => query.OrderByDescending(b => b.Name),
            _ => query.OrderBy(b => b.Name),
        };

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(normalizedSize).ToListAsync(cancellationToken);
        return (items, totalItems);
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
