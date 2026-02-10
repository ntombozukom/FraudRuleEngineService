using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlerts;
using FraudEngine.Domain.Common;
using FraudEngine.Domain.Enums;

namespace FraudEngine.API.Endpoints.FraudAlerts;

public static class GetFraudAlerts
{
    public static IEndpointRouteBuilder MapGetFraudAlerts(this IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.FraudAlerts, async (
            IMediator mediator,
            string? accountNumber,
            AlertStatus? status,
            AlertSeverity? severity,
            DateTime? from,
            DateTime? to,
            string? ruleName,
            int page = 1,
            int pageSize = 20) =>
        {
            var result = await mediator.Send(new GetFraudAlertsQuery
            {
                AccountNumber = accountNumber,
                Status = status,
                Severity = severity,
                From = from,
                To = to,
                RuleName = ruleName,
                Page = page,
                PageSize = pageSize
            });
            return Results.Ok(result);
        })
        .WithName("GetFraudAlerts")
        .WithTags(Tags.FraudAlerts)
        .Produces<PagedResponse<FraudAlertDto>>();

        return app;
    }
}
