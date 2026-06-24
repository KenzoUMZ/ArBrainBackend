using ArBrain.Application.Common;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Domain.Entities;
using ArBrain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Repositories;

public class TankRepository(AppDbContext context) : ITankRepository
{
    public async Task<(IReadOnlyList<Tank> Items, int TotalItems)> GetAllActiveAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        bool deletedOnly = false,
        CancellationToken cancellationToken = default)
    {
        var term = SearchTerm.Normalize(search)?.ToLowerInvariant();
        var sortField = SortQuery.NormalizeField(sortBy);
        var descending = SortQuery.IsDescending(sortDir);
        var (_, normalizedSize, skip) = PaginationQuery.Normalize(page, pageSize);

        var query = context.Tanks
            .AsNoTracking()
            .Where(t => deletedOnly ? !t.IsActive : t.IsActive);

        if (term is not null)
        {
            query = query.Where(t => t.Name.ToLower().Contains(term));
        }

        query = (sortField, descending) switch
        {
            ("capacity", false) => query.OrderBy(t => t.CapacityLiters).ThenByDescending(t => t.CreatedAt),
            ("capacity", true) => query.OrderByDescending(t => t.CapacityLiters).ThenByDescending(t => t.CreatedAt),
            ("name", false) => query.OrderBy(t => t.Name),
            ("name", true) => query.OrderByDescending(t => t.Name),
            ("createdat", false) => query.OrderBy(t => t.CreatedAt).ThenBy(t => t.Name),
            ("updatedat", false) => query.OrderBy(t => t.UpdatedAt ?? t.CreatedAt).ThenBy(t => t.Name),
            ("updatedat", true) => query.OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt).ThenBy(t => t.Name),
            ("createdat", true) => query.OrderByDescending(t => t.CreatedAt).ThenBy(t => t.Name),
            ("deletedat", false) => query.OrderBy(t => t.DeletedAt ?? t.UpdatedAt ?? t.CreatedAt).ThenBy(t => t.Name),
            ("deletedat", true) => query.OrderByDescending(t => t.DeletedAt ?? t.UpdatedAt ?? t.CreatedAt).ThenBy(t => t.Name),
            _ => deletedOnly
                ? query.OrderByDescending(t => t.DeletedAt ?? t.UpdatedAt ?? t.CreatedAt).ThenBy(t => t.Name)
                : query.OrderByDescending(t => t.CreatedAt).ThenBy(t => t.Name),
        };

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(normalizedSize).ToListAsync(cancellationToken);
        return (items, totalItems);
    }

    public async Task<Tank?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Tanks
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive, cancellationToken);
    }

    public async Task<Tank?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Tanks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
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
