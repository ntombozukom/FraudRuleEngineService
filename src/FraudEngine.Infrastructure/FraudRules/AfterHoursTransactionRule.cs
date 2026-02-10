using System.Text.Json;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.FraudRules;

public class AfterHoursTransactionRule : IFraudRule
{
    private readonly IFraudRuleConfigurationCache _configCache;

    public AfterHoursTransactionRule(IFraudRuleConfigurationCache configCache)
    {
        _configCache = configCache;
    }

    public string RuleName => "AfterHoursTransaction";

    public async Task<FraudRuleResult?> EvaluateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var config = await _configCache.GetByRuleNameAsync(RuleName, cancellationToken);
        if (config is null || !config.IsEnabled) return null;

        var parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(config.Parameters) ?? new();
        var startHour = parameters.TryGetValue("StartHour", out var startVal) ? startVal.GetInt32() : 23;
        var endHour = parameters.TryGetValue("EndHour", out var endVal) ? endVal.GetInt32() : 5;

        var hour = transaction.TransactionDate.Hour;
        bool isAfterHours = startHour > endHour
            ? (hour >= startHour || hour < endHour)
            : (hour >= startHour && hour < endHour);

        if (isAfterHours)
        {
            return new FraudRuleResult
            {
                RuleName = RuleName,
                Severity = AlertSeverity.Low,
                Description = $"Transaction at {transaction.TransactionDate:HH:mm} falls outside normal hours ({startHour}:00-{endHour}:00)."
            };
        }

        return null;
    }
}
