using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Interfaces;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlertStatistics;

public class GetFraudAlertStatisticsQueryHandler : IRequestHandler<GetFraudAlertStatisticsQuery, FraudAlertStatisticsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFraudAlertStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FraudAlertStatisticsDto> Handle(GetFraudAlertStatisticsQuery request, CancellationToken cancellationToken)
    {
        var (alerts, totalCount) = await _unitOfWork.FraudAlerts.GetAllAsync(
            page: 1, pageSize: int.MaxValue, cancellationToken: cancellationToken);

        var alertList = alerts.ToList();

        return new FraudAlertStatisticsDto
        {
            TotalAlerts = totalCount,
            BySeverity = alertList.GroupBy(a => a.Severity.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByStatus = alertList.GroupBy(a => a.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByRule = alertList.GroupBy(a => a.RuleName)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}
