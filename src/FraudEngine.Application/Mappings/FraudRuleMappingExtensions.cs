using FraudEngine.Application.DTOs;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Application.Mappings;

public static class FraudRuleMappingExtensions
{
    public static FraudRuleDto ToDto(this FraudRuleConfiguration config) => new()
    {
        Id = config.Id,
        RuleName = config.RuleName,
        IsEnabled = config.IsEnabled,
        Parameters = config.Parameters,
        Description = config.Description,
        CreatedAt = config.CreatedAt,
        CreatedBy = config.CreatedBy,
        UpdatedAt = config.UpdatedAt,
        LastModifiedBy = config.LastModifiedBy
    };

    public static IEnumerable<FraudRuleDto> ToDto(this IEnumerable<FraudRuleConfiguration> configs) =>
        configs.Select(c => c.ToDto());
}
