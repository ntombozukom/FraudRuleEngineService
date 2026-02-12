using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Common;
using FraudEngine.Domain.Enums;
using MediatR;

namespace FraudEngine.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<PagedResponse<AuditLogDto>>
{
    public string? EntityType { get; set; }
    public string? EntityName { get; set; }
    public string? ModifiedBy { get; set; }
    public AuditAction? Action { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
