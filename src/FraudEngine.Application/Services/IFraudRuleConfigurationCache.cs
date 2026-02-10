using FraudEngine.Domain.Entities;

namespace FraudEngine.Application.Services;

public interface IFraudRuleConfigurationCache
{
    Task<FraudRuleConfiguration?> GetByRuleNameAsync(string ruleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<FraudRuleConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
    void Invalidate();
    void Invalidate(string ruleName);
    Task RefreshAsync(string ruleName, CancellationToken cancellationToken = default);
}
