using MediatR;
using FraudEngine.API.Constants;
using FraudEngine.Application.DTOs;
using FraudEngine.Application.Features.AuditLogs.Queries.GetAuditLogs;
using FraudEngine.Domain.Common;
using FraudEngine.Domain.Enums;

namespace FraudEngine.API.Endpoints.AuditLogs;

public static class GetAuditLogs
{
    public static IEndpointRouteBuilder MapGetAuditLogs(this IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.AuditLogs, async (
            string? entityType,
            string? entityName,
            string? modifiedBy,
            AuditAction? action,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAuditLogsQuery
            {
                EntityType = entityType,
                EntityName = entityName,
                ModifiedBy = modifiedBy,
                Action = action,
                From = from,
                To = to,
                Page = page > 0 ? page : 1,
                PageSize = pageSize > 0 ? pageSize : 20
            });
            return Results.Ok(result);
        })
        .WithName("GetAuditLogs")
        .WithTags(Tags.AuditLogs)
        .Produces<PagedResponse<AuditLogDto>>();

        return app;
    }
}
