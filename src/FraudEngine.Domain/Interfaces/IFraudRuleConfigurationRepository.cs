using FraudEngine.Domain.Entities;

namespace FraudEngine.Domain.Interfaces;

public interface IFraudRuleConfigurationRepository
{
    Task<IEnumerable<FraudRuleConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FraudRuleConfiguration?> GetByRuleNameAsync(string ruleName, CancellationToken cancellationToken = default);
    Task UpdateAsync(FraudRuleConfiguration config, CancellationToken cancellationToken = default);
    Task AddAsync(FraudRuleConfiguration config, CancellationToken cancellationToken = default);
}
