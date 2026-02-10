using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.FraudRules.Commands.UpdateFraudRule;

namespace FraudEngine.API.Endpoints.FraudRules;

public static class UpdateFraudRule
{
    public static IEndpointRouteBuilder MapUpdateFraudRule(this IEndpointRouteBuilder app)
    {
        app.MapPut($"{Routes.FraudRules}/{{ruleName}}", async (
            string ruleName,
            UpdateFraudRuleRequest request,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateFraudRuleCommand
            {
                RuleName = ruleName,
                IsEnabled = request.IsEnabled,
                Parameters = request.Parameters
            });
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("UpdateFraudRule")
        .WithTags(Tags.FraudRules)
        .Produces<FraudRuleDto>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
