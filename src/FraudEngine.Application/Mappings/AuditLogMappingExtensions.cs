using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Application.Mappings;

public static class AuditLogMappingExtensions
{
    public static AuditLogDto ToDto(this AuditLog auditLog) => new()
    {
        Id = auditLog.Id,
        EntityType = auditLog.EntityType,
        EntityId = auditLog.EntityId,
        EntityName = auditLog.EntityName,
        Action = auditLog.Action.ToString(),
        PropertyName = auditLog.PropertyName,
        OldValue = auditLog.OldValue,
        NewValue = auditLog.NewValue,
        ModifiedBy = auditLog.ModifiedBy,
        ModifiedAt = auditLog.ModifiedAt
    };

    public static IEnumerable<AuditLogDto> ToDto(this IEnumerable<AuditLog> auditLogs) =>
        auditLogs.Select(a => a.ToDto());
}
