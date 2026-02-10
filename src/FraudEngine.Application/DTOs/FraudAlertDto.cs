namespace FraudEngine.Application.DTOs;

public class FraudAlertDto
{
    public Guid AlertReference { get; set; }
    public Guid TransactionReference { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public TransactionDto? Transaction { get; set; }
}
