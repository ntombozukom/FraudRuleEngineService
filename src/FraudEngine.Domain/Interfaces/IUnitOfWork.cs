namespace FraudEngine.Domain.Interfaces;

public interface IUnitOfWork
{
    ITransactionRepository Transactions { get; }
    IFraudAlertRepository FraudAlerts { get; }
    IFraudRuleConfigurationRepository FraudRuleConfigurations { get; }
    IAuditLogRepository AuditLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
