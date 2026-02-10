using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.FraudRules.Queries.GetFraudRules;

namespace FraudEngine.API.Endpoints.FraudRules;

public static class GetFraudRules
{
    public static IEndpointRouteBuilder MapGetFraudRules(this IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.FraudRules, async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetFraudRulesQuery());
            return Results.Ok(result);
        })
        .WithName("GetFraudRules")
        .WithTags(Tags.FraudRules)
        .Produces<IEnumerable<FraudRuleDto>>();

        return app;
    }
}
