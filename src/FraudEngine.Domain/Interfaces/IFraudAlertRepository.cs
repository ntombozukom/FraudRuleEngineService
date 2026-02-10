using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;

namespace FraudEngine.Domain.Interfaces;

public interface IFraudAlertRepository
{
    Task<FraudAlert?> GetByAlertReferenceAsync(Guid alertReference, CancellationToken cancellationToken = default);
    Task<(IEnumerable<FraudAlert> Items, int TotalCount)> GetAllAsync(
        string? accountNumber = null,
        AlertStatus? status = null,
        AlertSeverity? severity = null,
        DateTime? from = null,
        DateTime? to = null,
        string? ruleName = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    Task<FraudAlertStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<FraudAlert> alerts, CancellationToken cancellationToken = default);
    Task UpdateAsync(FraudAlert alert, CancellationToken cancellationToken = default);
}

public record FraudAlertStatistics(int TotalAlerts, int OpenAlerts, int ReviewedAlerts, int DismissedAlerts, Dictionary<string, int> AlertsByRule);
