using FraudEngine.Application.DTOs;
using MediatR;

namespace FraudEngine.Application.Features.FraudRules.Queries.GetFraudRules;

public record GetFraudRulesQuery : IRequest<IEnumerable<FraudRuleDto>>;
