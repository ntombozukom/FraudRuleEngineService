using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlertStatistics;

namespace FraudEngine.API.Endpoints.FraudAlerts;

public static class GetFraudAlertStatistics
{
    public static IEndpointRouteBuilder MapGetFraudAlertStatistics(this IEndpointRouteBuilder app)
    {
        app.MapGet($"{Routes.FraudAlerts}/statistics", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetFraudAlertStatisticsQuery());
            return Results.Ok(result);
        })
        .WithName("GetFraudAlertStatistics")
        .WithTags(Tags.FraudAlerts)
        .Produces<FraudAlertStatisticsDto>();

        return app;
    }
}
