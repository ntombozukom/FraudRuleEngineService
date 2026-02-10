namespace FraudEngine.Application.DTOs;

public class EvaluationResultDto
{
    public Guid TransactionReference { get; set; }
    public bool IsFlagged { get; set; }
    public int AlertsGenerated { get; set; }
    public List<FraudAlertDto> Alerts { get; set; } = new();
}
