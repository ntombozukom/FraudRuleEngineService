using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Enums;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Commands.ReviewFraudAlert;

public class ReviewFraudAlertCommand : IRequest<FraudAlertDto?>
{
    public Guid AlertReference { get; set; }
    public AlertStatus Status { get; set; }
    public string ReviewedBy { get; set; } = string.Empty;
}
