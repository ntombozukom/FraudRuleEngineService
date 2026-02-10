using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Common;
using FraudEngine.Domain.Enums;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlerts;

public class GetFraudAlertsQuery : IRequest<PagedResponse<FraudAlertDto>>
{
    public string? AccountNumber { get; set; }
    public AlertStatus? Status { get; set; }
    public AlertSeverity? Severity { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? RuleName { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
