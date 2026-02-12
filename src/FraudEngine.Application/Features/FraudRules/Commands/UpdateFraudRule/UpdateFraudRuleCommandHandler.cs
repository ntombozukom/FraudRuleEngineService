using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Enums;
using FraudEngine.Domain.Interfaces;
using MediatR;

namespace FraudEngine.Application.Features.FraudRules.Commands.UpdateFraudRule;

public class UpdateFraudRuleCommandHandler : IRequestHandler<UpdateFraudRuleCommand, FraudRuleDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFraudRuleConfigurationCache _cache;

    public UpdateFraudRuleCommandHandler(IUnitOfWork unitOfWork, IFraudRuleConfigurationCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<FraudRuleDto?> Handle(UpdateFraudRuleCommand request, CancellationToken cancellationToken)
    {
        var config = await _unitOfWork.FraudRuleConfigurations.GetByRuleNameAsync(request.RuleName, cancellationToken);
        if (config is null) return null;

        var auditLogs = new List<AuditLog>();

        if (request.IsEnabled.HasValue && config.IsEnabled != request.IsEnabled.Value)
        {
            auditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = nameof(FraudRuleConfiguration),
                EntityId = config.Id.ToString(),
                EntityName = config.RuleName,
                Action = request.IsEnabled.Value ? AuditAction.Enabled : AuditAction.Disabled,
                PropertyName = nameof(config.IsEnabled),
                OldValue = config.IsEnabled.ToString(),
                NewValue = request.IsEnabled.Value.ToString(),
                ModifiedBy = request.ModifiedBy,
                ModifiedAt = DateTime.UtcNow
            });

            config.IsEnabled = request.IsEnabled.Value;
        }

        if (request.Parameters is not null && config.Parameters != request.Parameters)
        {
            auditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = nameof(FraudRuleConfiguration),
                EntityId = config.Id.ToString(),
                EntityName = config.RuleName,
                Action = AuditAction.Updated,
                PropertyName = nameof(config.Parameters),
                OldValue = config.Parameters,
                NewValue = request.Parameters,
                ModifiedBy = request.ModifiedBy,
                ModifiedAt = DateTime.UtcNow
            });

            config.Parameters = request.Parameters;
        }

        if (auditLogs.Count > 0)
        {
            config.UpdatedAt = DateTime.UtcNow;
            config.LastModifiedBy = request.ModifiedBy;

            await _unitOfWork.FraudRuleConfigurations.UpdateAsync(config, cancellationToken);

            foreach (var auditLog in auditLogs)
            {
                await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RefreshAsync(request.RuleName, cancellationToken);
        }

        return config.ToDto();
    }
}
