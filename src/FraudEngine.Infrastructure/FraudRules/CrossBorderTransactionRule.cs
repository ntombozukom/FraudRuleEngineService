using System.Text.Json;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.FraudRules;

public class CrossBorderTransactionRule : IFraudRule
{
    private readonly IFraudRuleConfigurationCache _configCache;

    public CrossBorderTransactionRule(IFraudRuleConfigurationCache configCache)
    {
        _configCache = configCache;
    }

    public string RuleName => "CrossBorderTransaction";

    public async Task<FraudRuleResult?> EvaluateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var config = await _configCache.GetByRuleNameAsync(RuleName, cancellationToken);
        if (config is null || !config.IsEnabled) return null;

        var parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(config.Parameters) ?? new();
        var homeCountry = parameters.TryGetValue("HomeCountry", out var val) ? val.GetString() ?? "ZA" : "ZA";

        if (!string.Equals(transaction.Country, homeCountry, StringComparison.OrdinalIgnoreCase))
        {
            return new FraudRuleResult
            {
                RuleName = RuleName,
                Severity = AlertSeverity.Medium,
                Description = $"Transaction originated from {transaction.Country}, outside home country {homeCountry}."
            };
        }

        return null;
    }
}
