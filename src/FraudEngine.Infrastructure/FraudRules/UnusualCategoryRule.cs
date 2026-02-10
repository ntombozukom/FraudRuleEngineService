using System.Text.Json;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.FraudRules;

public class UnusualCategoryRule : IFraudRule
{
    private readonly IFraudRuleConfigurationCache _configCache;
    private readonly ITransactionRepository _transactionRepo;

    public UnusualCategoryRule(IFraudRuleConfigurationCache configCache, ITransactionRepository transactionRepo)
    {
        _configCache = configCache;
        _transactionRepo = transactionRepo;
    }

    public string RuleName => "UnusualCategory";

    public async Task<FraudRuleResult?> EvaluateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var config = await _configCache.GetByRuleNameAsync(RuleName, cancellationToken);
        if (config is null || !config.IsEnabled) return null;

        var parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(config.Parameters) ?? new();
        var minHistoryCount = parameters.TryGetValue("MinHistoryCount", out var minVal) ? minVal.GetInt32() : 10;
        var unusualThreshold = parameters.TryGetValue("UnusualThresholdPercent", out var threshVal) ? threshVal.GetDouble() : 5.0;

        var history = (await _transactionRepo.GetByAccountNumberAsync(transaction.AccountNumber, cancellationToken: cancellationToken)).ToList();

        if (history.Count < minHistoryCount) return null;

        var categoryCount = history.Count(t => t.Category == transaction.Category);
        var categoryPercentage = (double)categoryCount / history.Count * 100;

        if (categoryPercentage < unusualThreshold)
        {
            return new FraudRuleResult
            {
                RuleName = RuleName,
                Severity = AlertSeverity.Low,
                Description = $"Category '{transaction.Category}' represents only {categoryPercentage:F1}% of account history (threshold: {unusualThreshold}%)."
            };
        }

        return null;
    }
}
