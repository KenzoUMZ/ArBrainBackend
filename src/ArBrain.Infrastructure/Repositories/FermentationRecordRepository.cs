using ArBrain.Application.Common;
using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Domain.Entities;
using ArBrain.Domain.Enums;
using ArBrain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArBrain.Infrastructure.Repositories;

public class FermentationRecordRepository(AppDbContext context) : IFermentationRecordRepository
{
    public async Task<(IReadOnlyList<FermentationRecord> Items, int TotalItems)> GetAllAsync(
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        FermentationComplianceStatus? complianceStatus = null,
        int page = 1,
        int pageSize = PaginationQuery.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var term = SearchTerm.Normalize(search)?.ToLowerInvariant();
        var sortField = SortQuery.NormalizeField(sortBy);
        var descending = SortQuery.IsDescending(sortDir);
        var (_, normalizedSize, skip) = PaginationQuery.Normalize(page, pageSize);

        var query = QueryWithRelations();

        if (term is not null)
        {
            query = query.Where(r =>
                r.BatchNumber.ToLower().Contains(term) ||
                r.Beer.Name.ToLower().Contains(term) ||
                r.Tank.Name.ToLower().Contains(term) ||
                (r.Observations != null && r.Observations.ToLower().Contains(term)));
        }

        if (complianceStatus.HasValue)
        {
            query = query.Where(r => r.ComplianceStatus == complianceStatus.Value);
        }

        query = (sortField, descending) switch
        {
            ("batchnumber", false) => query.OrderBy(r => r.BatchNumber).ThenByDescending(r => r.RegisteredAt),
            ("batchnumber", true) => query.OrderByDescending(r => r.BatchNumber).ThenByDescending(r => r.RegisteredAt),
            ("beername", false) => query.OrderBy(r => r.Beer.Name).ThenByDescending(r => r.RegisteredAt),
            ("beername", true) => query.OrderByDescending(r => r.Beer.Name).ThenByDescending(r => r.RegisteredAt),
            ("tankname", false) => query.OrderBy(r => r.Tank.Name).ThenByDescending(r => r.RegisteredAt),
            ("tankname", true) => query.OrderByDescending(r => r.Tank.Name).ThenByDescending(r => r.RegisteredAt),
            ("temperature", false) => query.OrderBy(r => r.Temperature).ThenByDescending(r => r.RegisteredAt),
            ("temperature", true) => query.OrderByDescending(r => r.Temperature).ThenByDescending(r => r.RegisteredAt),
            ("ph", false) => query.OrderBy(r => r.Ph).ThenByDescending(r => r.RegisteredAt),
            ("ph", true) => query.OrderByDescending(r => r.Ph).ThenByDescending(r => r.RegisteredAt),
            ("extract", false) => query.OrderBy(r => r.Extract).ThenByDescending(r => r.RegisteredAt),
            ("extract", true) => query.OrderByDescending(r => r.Extract).ThenByDescending(r => r.RegisteredAt),
            ("compliancestatus", false) => query.OrderBy(r => r.ComplianceStatus).ThenByDescending(r => r.RegisteredAt),
            ("compliancestatus", true) => query.OrderByDescending(r => r.ComplianceStatus).ThenByDescending(r => r.RegisteredAt),
            ("registeredat", false) => query.OrderBy(r => r.RegisteredAt),
            ("registeredat", true) => query.OrderByDescending(r => r.RegisteredAt),
            _ => query.OrderByDescending(r => r.RegisteredAt),
        };

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(normalizedSize).ToListAsync(cancellationToken);
        return (items, totalItems);
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
        string? search = null,
        string? sortBy = null,
        string? sortDir = null,
        CancellationToken cancellationToken = default)
    {
        var term = SearchTerm.Normalize(search)?.ToLowerInvariant();
        var sortField = SortQuery.NormalizeField(sortBy);
        var descending = SortQuery.IsDescending(sortDir);

        var records = context.FermentationRecords
            .AsNoTracking()
            .Include(r => r.Beer)
            .AsQueryable();

        if (term is not null)
        {
            records = records.Where(r =>
                r.BatchNumber.ToLower().Contains(term) ||
                r.Beer.Name.ToLower().Contains(term));
        }

        var grouped = records
            .GroupBy(r => new { r.BatchNumber, BeerName = r.Beer.Name })
            .Select(g => new
            {
                g.Key.BatchNumber,
                g.Key.BeerName,
                Count = g.Count(),
            });

        grouped = (sortField, descending) switch
        {
            ("beername", false) => grouped.OrderBy(item => item.BeerName).ThenByDescending(item => item.BatchNumber),
            ("beername", true) => grouped.OrderByDescending(item => item.BeerName).ThenByDescending(item => item.BatchNumber),
            ("recordcount", false) => grouped.OrderBy(item => item.Count).ThenByDescending(item => item.BatchNumber),
            ("recordcount", true) => grouped.OrderByDescending(item => item.Count).ThenByDescending(item => item.BatchNumber),
            ("batchnumber", false) => grouped.OrderBy(item => item.BatchNumber),
            ("batchnumber", true) => grouped.OrderByDescending(item => item.BatchNumber),
            _ => grouped.OrderByDescending(item => item.BatchNumber),
        };

        var rows = await grouped.ToListAsync(cancellationToken);

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
