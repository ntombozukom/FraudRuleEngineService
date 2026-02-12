using FraudEngine.Domain.Common;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;

namespace FraudEngine.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, string entityId, CancellationToken cancellationToken = default);
    Task<PagedResponse<AuditLog>> GetAllAsync(
        string? entityType = null,
        string? entityName = null,
        string? modifiedBy = null,
        AuditAction? action = null,
        DateTime? from = null,
        DateTime? to = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
