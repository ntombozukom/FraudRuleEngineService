using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.FraudAlerts.Commands.ReviewFraudAlert;

namespace FraudEngine.API.Endpoints.FraudAlerts;

public static class ReviewFraudAlert
{
    public static IEndpointRouteBuilder MapReviewFraudAlert(this IEndpointRouteBuilder app)
    {
        app.MapPatch($"{Routes.FraudAlerts}/{{alertReference:guid}}/review", async (
            Guid alertReference,
            ReviewAlertRequest request,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new ReviewFraudAlertCommand
            {
                AlertReference = alertReference,
                Status = request.Status,
                ReviewedBy = request.ReviewedBy
            });
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("ReviewFraudAlert")
        .WithTags(Tags.FraudAlerts)
        .Produces<FraudAlertDto>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
