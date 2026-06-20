using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using ArBrain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Repositories;

public class FermentationRecordRepository(AppDbContext context) : IFermentationRecordRepository
{
    public async Task<IReadOnlyList<FermentationRecord>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await QueryWithRelations()
            .OrderByDescending(r => r.RegisteredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<FermentationRecord?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await QueryWithRelations()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FermentationRecord>> GetByBatchNumberAsync(
        string batchNumber,
        CancellationToken cancellationToken = default)
    {
        return await QueryWithRelations()
            .Where(r => r.BatchNumber == batchNumber)
            .OrderBy(r => r.RegisteredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<(string BatchNumber, string BeerName, int Count)>> GetBatchSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await context.FermentationRecords
            .AsNoTracking()
            .Include(r => r.Beer)
            .GroupBy(r => new { r.BatchNumber, BeerName = r.Beer.Name })
            .Select(g => new
            {
                g.Key.BatchNumber,
                g.Key.BeerName,
                Count = g.Count(),
            })
            .OrderBy(item => item.BatchNumber)
            .ToListAsync(cancellationToken);

        return rows
            .Select(row => (row.BatchNumber, row.BeerName, row.Count))
            .ToList();
    }

    public async Task<Dictionary<FermentationComplianceStatus, int>> GetComplianceCountsAsync(
        CancellationToken cancellationToken = default)
    {
        var grouped = await context.FermentationRecords
            .AsNoTracking()
            .GroupBy(r => r.ComplianceStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return grouped.ToDictionary(x => x.Status, x => x.Count);
    }

    public async Task AddAsync(FermentationRecord record, CancellationToken cancellationToken = default)
    {
        context.FermentationRecords.Add(record);
        await context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<FermentationRecord> QueryWithRelations()
    {
        return context.FermentationRecords
            .AsNoTracking()
            .Include(r => r.Beer)
            .Include(r => r.Tank);
    }
}
