using FraudEngine.Domain.Enums;

namespace FraudEngine.Domain.Entities;

public class FraudAlert
{
    public int Id { get; set; }
    public Guid AlertReference { get; set; } = Guid.NewGuid();
    public int TransactionId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public AlertStatus Status { get; set; } = AlertStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public Transaction Transaction { get; set; } = null!;
}
