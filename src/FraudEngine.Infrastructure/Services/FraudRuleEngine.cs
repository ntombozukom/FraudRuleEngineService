using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FraudEngine.Infrastructure.Services;

public class FraudRuleEngine : IFraudRuleEngine
{
    private readonly IEnumerable<IFraudRule> _rules;
    private readonly ILogger<FraudRuleEngine> _logger;

    public FraudRuleEngine(IEnumerable<IFraudRule> rules, ILogger<FraudRuleEngine> logger)
    {
        _rules = rules;
        _logger = logger;
    }

    public async Task<IEnumerable<FraudRuleResult>> EvaluateTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var results = new List<FraudRuleResult>();

        foreach (var rule in _rules)
        {
            try
            {
                var result = await rule.EvaluateAsync(transaction, cancellationToken);
                if (result is not null)
                {
                    results.Add(result);
                    _logger.LogInformation("Rule {RuleName} triggered for transaction {TransactionId}", rule.RuleName, transaction.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating rule {RuleName} for transaction {TransactionId}", rule.RuleName, transaction.Id);
            }
        }

        return results;
    }
}
