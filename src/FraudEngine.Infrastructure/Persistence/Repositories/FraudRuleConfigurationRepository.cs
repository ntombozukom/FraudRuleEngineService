using Microsoft.EntityFrameworkCore;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.Persistence.Repositories;

public class FraudRuleConfigurationRepository : IFraudRuleConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public FraudRuleConfigurationRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<FraudRuleConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FraudRuleConfigurations
            .OrderBy(r => r.RuleName)
            .ToListAsync(cancellationToken);
    }

    public async Task<FraudRuleConfiguration?> GetByRuleNameAsync(string ruleName, CancellationToken cancellationToken = default)
    {
        return await _context.FraudRuleConfigurations
            .FirstOrDefaultAsync(r => r.RuleName == ruleName, cancellationToken);
    }

    public Task UpdateAsync(FraudRuleConfiguration config, CancellationToken cancellationToken = default)
    {
        _context.FraudRuleConfigurations.Update(config);
        return Task.CompletedTask;
    }

    public async Task AddAsync(FraudRuleConfiguration config, CancellationToken cancellationToken = default)
    {
        await _context.FraudRuleConfigurations.AddAsync(config, cancellationToken);
    }
}
