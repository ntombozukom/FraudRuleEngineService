using FraudEngine.Domain.Enums;

namespace FraudEngine.Application.DTOs;

public class ReviewAlertRequest
{
    public AlertStatus Status { get; set; }
    public string ReviewedBy { get; set; } = string.Empty;
}
