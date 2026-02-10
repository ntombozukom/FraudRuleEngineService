using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.Transactions.Commands.EvaluateTransaction;

namespace FraudEngine.API.Endpoints.Transactions;

public static class EvaluateTransaction
{
    public static IEndpointRouteBuilder MapEvaluateTransaction(this IEndpointRouteBuilder app)
    {
        app.MapPost($"{Routes.Transactions}/evaluate", async (
            EvaluateTransactionCommand command,
            IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("EvaluateTransaction")
        .WithTags(Tags.Transactions)
        .Produces<EvaluationResultDto>()
        .ProducesValidationProblem();

        return app;
    }
}
