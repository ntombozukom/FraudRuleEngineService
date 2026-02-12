using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Transactions = new TransactionRepository(context);
        FraudAlerts = new FraudAlertRepository(context);
        FraudRuleConfigurations = new FraudRuleConfigurationRepository(context);
        AuditLogs = new AuditLogRepository(context);
    }

    public ITransactionRepository Transactions { get; }
    public IFraudAlertRepository FraudAlerts { get; }
    public IFraudRuleConfigurationRepository FraudRuleConfigurations { get; }
    public IAuditLogRepository AuditLogs { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
