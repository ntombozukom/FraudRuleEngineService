using System.Text.Json;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.FraudRules;

public class RapidSuccessiveTransactionsRule : IFraudRule
{
    private readonly IFraudRuleConfigurationCache _configCache;
    private readonly ITransactionRepository _transactionRepo;

    public RapidSuccessiveTransactionsRule(IFraudRuleConfigurationCache configCache, ITransactionRepository transactionRepo)
    {
        _configCache = configCache;
        _transactionRepo = transactionRepo;
    }

    public string RuleName => "RapidSuccessiveTransactions";

    public async Task<FraudRuleResult?> EvaluateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var config = await _configCache.GetByRuleNameAsync(RuleName, cancellationToken);
        if (config is null || !config.IsEnabled) return null;

        var parameters = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(config.Parameters) ?? new();
        var maxTransactions = parameters.TryGetValue("MaxTransactions", out var maxVal) ? maxVal.GetInt32() : 3;
        var timeWindowMinutes = parameters.TryGetValue("TimeWindowMinutes", out var timeVal) ? timeVal.GetInt32() : 5;

        var windowStart = transaction.TransactionDate.AddMinutes(-timeWindowMinutes);
        var recentTransactions = await _transactionRepo.GetByAccountNumberAsync(
            transaction.AccountNumber, windowStart, transaction.TransactionDate, cancellationToken);

        // Count excludes the current transaction (it may not be persisted yet, but check just in case)
        var count = recentTransactions.Count(t => t.Id != transaction.Id);

        if (count >= maxTransactions)
        {
            return new FraudRuleResult
            {
                RuleName = RuleName,
                Severity = AlertSeverity.Medium,
                Description = $"Account has {count + 1} transactions within {timeWindowMinutes} minutes (threshold: {maxTransactions})."
            };
        }

        return null;
    }
}
