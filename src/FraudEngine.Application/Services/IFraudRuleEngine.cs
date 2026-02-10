using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Application.Services;

public interface IFraudRuleEngine
{
    Task<IEnumerable<FraudRuleResult>> EvaluateTransactionAsync(Domain.Entities.Transaction transaction, CancellationToken cancellationToken = default);
}
