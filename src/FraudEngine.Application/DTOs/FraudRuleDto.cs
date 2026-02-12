namespace FraudEngine.Application.DTOs;

public class FraudRuleDto
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string Parameters { get; set; } = "{}";
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
