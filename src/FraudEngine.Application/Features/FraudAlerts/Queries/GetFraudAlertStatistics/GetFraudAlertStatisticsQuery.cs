using FraudEngine.Application.DTOs;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlertStatistics;

public record GetFraudAlertStatisticsQuery : IRequest<FraudAlertStatisticsDto>;
