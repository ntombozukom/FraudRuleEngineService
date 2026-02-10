using FraudEngine.Application.DTOs;
using FraudEngine.Application.Mappings;
using FraudEngine.Application.Services;
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

        if (request.IsEnabled.HasValue)
            config.IsEnabled = request.IsEnabled.Value;

        if (request.Parameters is not null)
            config.Parameters = request.Parameters;

        config.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.FraudRuleConfigurations.UpdateAsync(config, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.RefreshAsync(request.RuleName, cancellationToken);

        return config.ToDto();
    }
}
