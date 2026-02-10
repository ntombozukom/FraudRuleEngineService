using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Domain.Common;
using FraudEngine.Domain.Interfaces;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlerts;

public class GetFraudAlertsQueryHandler : IRequestHandler<GetFraudAlertsQuery, PagedResponse<FraudAlertDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFraudAlertsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<FraudAlertDto>> Handle(GetFraudAlertsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.FraudAlerts.GetAllAsync(
            accountNumber: request.AccountNumber,
            status: request.Status,
            severity: request.Severity,
            from: request.From,
            to: request.To,
            ruleName: request.RuleName,
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        return new PagedResponse<FraudAlertDto>
        {
            Items = items.ToDto(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
