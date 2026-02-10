using Microsoft.EntityFrameworkCore;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.Persistence.Repositories;

public class FraudAlertRepository : IFraudAlertRepository
{
    private readonly ApplicationDbContext _context;

    public FraudAlertRepository(ApplicationDbContext context) => _context = context;

    public async Task<FraudAlert?> GetByAlertReferenceAsync(Guid alertReference, CancellationToken cancellationToken = default)
    {
        return await _context.FraudAlerts
            .Include(a => a.Transaction)
            .FirstOrDefaultAsync(a => a.AlertReference == alertReference, cancellationToken);
    }

    public async Task<(IEnumerable<FraudAlert> Items, int TotalCount)> GetAllAsync(
        string? accountNumber = null,
        AlertStatus? status = null,
        AlertSeverity? severity = null,
        DateTime? from = null,
        DateTime? to = null,
        string? ruleName = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.FraudAlerts
            .Include(a => a.Transaction)
            .AsQueryable();

        if (!string.IsNullOrEmpty(accountNumber))
            query = query.Where(a => a.Transaction.AccountNumber == accountNumber);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (severity.HasValue)
            query = query.Where(a => a.Severity == severity.Value);

        if (from.HasValue)
            query = query.Where(a => a.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.CreatedAt <= to.Value);

        if (!string.IsNullOrWhiteSpace(ruleName))
            query = query.Where(a => a.RuleName == ruleName);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<FraudAlertStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var alerts = await _context.FraudAlerts.ToListAsync(cancellationToken);

        return new FraudAlertStatistics(
            TotalAlerts: alerts.Count,
            OpenAlerts: alerts.Count(a => a.Status == AlertStatus.Open),
            ReviewedAlerts: alerts.Count(a => a.Status == AlertStatus.Reviewed),
            DismissedAlerts: alerts.Count(a => a.Status == AlertStatus.Dismissed),
            AlertsByRule: alerts.GroupBy(a => a.RuleName).ToDictionary(g => g.Key, g => g.Count())
        );
    }

    public async Task AddRangeAsync(IEnumerable<FraudAlert> alerts, CancellationToken cancellationToken = default)
    {
        await _context.FraudAlerts.AddRangeAsync(alerts, cancellationToken);
    }

    public Task UpdateAsync(FraudAlert alert, CancellationToken cancellationToken = default)
    {
        _context.FraudAlerts.Update(alert);
        return Task.CompletedTask;
    }
}
