namespace FraudEngine.Domain.Entities;

public class FraudRuleConfiguration
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Parameters { get; set; } = "{}";
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
