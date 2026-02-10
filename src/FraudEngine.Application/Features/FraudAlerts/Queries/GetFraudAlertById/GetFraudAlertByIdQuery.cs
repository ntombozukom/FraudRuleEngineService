using FraudEngine.Application.DTOs;
using MediatR;

namespace FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlertById;

public record GetFraudAlertByReferenceQuery(Guid AlertReference) : IRequest<FraudAlertDto?>;
