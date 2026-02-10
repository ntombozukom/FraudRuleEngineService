using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Domain.Interfaces;
using MediatR;

namespace FraudEngine.Application.Features.FraudRules.Queries.GetFraudRules;

public class GetFraudRulesQueryHandler : IRequestHandler<GetFraudRulesQuery, IEnumerable<FraudRuleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFraudRulesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FraudRuleDto>> Handle(GetFraudRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = await _unitOfWork.FraudRuleConfigurations.GetAllAsync(cancellationToken);
        return rules.ToDto();
    }
}
