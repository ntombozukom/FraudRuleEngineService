using System.Text.Json;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.FraudRules;

public class HighValueTransactionRule : IFraudRule
{
    private readonly IFraudRuleConfigurationCache _configCache;

    public HighValueTransactionRule(IFraudRuleConfigurationCache configCache)
    {
        _configCache = configCache;
    }

    public string RuleName => "HighValueTransaction";

    public async Task<FraudRuleResult?> EvaluateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var config = await _configCache.GetByRuleNameAsync(RuleName, cancellationToken);
        if (config is null || !config.IsEnabled) return null;

        var parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(config.Parameters) ?? new();
        var threshold = parameters.TryGetValue("ThresholdAmount", out var val) ? val.GetDecimal() : 50000m;

        if (Math.Abs(transaction.Amount) > threshold)
        {
            return new FraudRuleResult
            {
                RuleName = RuleName,
                Severity = AlertSeverity.High,
                Description = $"Transaction amount R{Math.Abs(transaction.Amount):N2} exceeds threshold of R{threshold:N2}."
            };
        }

        return null;
    }
}
