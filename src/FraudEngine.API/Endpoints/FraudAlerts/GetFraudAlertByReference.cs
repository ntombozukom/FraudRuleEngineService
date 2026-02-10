using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.FraudAlerts.Queries.GetFraudAlertById;

namespace FraudEngine.API.Endpoints.FraudAlerts;

public static class GetFraudAlertByReference
{
    public static IEndpointRouteBuilder MapGetFraudAlertByReference(this IEndpointRouteBuilder app)
    {
        app.MapGet($"{Routes.FraudAlerts}/{{alertReference:guid}}", async (Guid alertReference, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetFraudAlertByReferenceQuery(alertReference));
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetFraudAlertByReference")
        .WithTags(Tags.FraudAlerts)
        .Produces<FraudAlertDto>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
