using FraudEngine.Application.DTOs;
using MediatR;

namespace FraudEngine.Application.Features.FraudRules.Commands.UpdateFraudRule;

public class UpdateFraudRuleCommand : IRequest<FraudRuleDto?>
{
    public string RuleName { get; set; } = string.Empty;
    public bool? IsEnabled { get; set; }
    public string? Parameters { get; set; }
}
