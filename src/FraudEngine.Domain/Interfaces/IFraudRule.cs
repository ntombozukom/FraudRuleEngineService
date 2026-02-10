namespace FraudEngine.Domain.Interfaces;

public interface IFraudRule
{
    string RuleName { get; }
    Task<FraudRuleResult?> EvaluateAsync(Entities.Transaction transaction, CancellationToken cancellationToken = default);
}

public class FraudRuleResult
{
    public string RuleName { get; set; } = string.Empty;
    public Enums.AlertSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
}
