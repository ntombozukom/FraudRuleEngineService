using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Domain.Common;
using FraudEngine.Domain.Interfaces;
using MediatR;

namespace FraudEngine.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedResponse<AuditLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuditLogsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.AuditLogs.GetAllAsync(
            entityType: request.EntityType,
            entityName: request.EntityName,
            modifiedBy: request.ModifiedBy,
            action: request.Action,
            from: request.From,
            to: request.To,
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        return new PagedResponse<AuditLogDto>
        {
            Items = result.Items.ToDto().ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
