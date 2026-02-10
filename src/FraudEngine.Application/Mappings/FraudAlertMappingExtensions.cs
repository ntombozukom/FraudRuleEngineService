using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Application.Mappings;

public static class FraudAlertMappingExtensions
{
    public static FraudAlertDto ToDto(this FraudAlert alert) => new()
    {
        AlertReference = alert.AlertReference,
        TransactionReference = alert.Transaction.TransactionReference,
        RuleName = alert.RuleName,
        Severity = alert.Severity.ToString(),
        Description = alert.Description,
        Status = alert.Status.ToString(),
        CreatedAt = alert.CreatedAt,
        ReviewedBy = alert.ReviewedBy,
        ReviewedAt = alert.ReviewedAt,
        Transaction = alert.Transaction.ToDto()
    };

    public static FraudAlertDto ToDtoWithoutTransaction(this FraudAlert alert) => new()
    {
        AlertReference = alert.AlertReference,
        TransactionReference = alert.Transaction.TransactionReference,
        RuleName = alert.RuleName,
        Severity = alert.Severity.ToString(),
        Description = alert.Description,
        Status = alert.Status.ToString(),
        CreatedAt = alert.CreatedAt,
        ReviewedBy = alert.ReviewedBy,
        ReviewedAt = alert.ReviewedAt
    };

    public static IEnumerable<FraudAlertDto> ToDto(this IEnumerable<FraudAlert> alerts) =>
        alerts.Select(a => a.ToDto());
}
