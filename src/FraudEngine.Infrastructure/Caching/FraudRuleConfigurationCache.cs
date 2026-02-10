using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Entities;
using FraudEngine.Domain.Interfaces;

namespace FraudEngine.Infrastructure.Caching;

public class FraudRuleConfigurationCache : IFraudRuleConfigurationCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<FraudRuleConfigurationCache> _logger;

    private const string AllRulesCacheKey = "FraudRules:All";
    private const string RuleCacheKeyPrefix = "FraudRules:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    private static readonly SemaphoreSlim _lock = new(1, 1);

    public FraudRuleConfigurationCache(
        IServiceScopeFactory scopeFactory,
        IMemoryCache cache,
        ILogger<FraudRuleConfigurationCache> logger)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<FraudRuleConfiguration?> GetByRuleNameAsync(string ruleName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{RuleCacheKeyPrefix}{ruleName}";

        if (_cache.TryGetValue(cacheKey, out FraudRuleConfiguration? cached))
        {
            return cached;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(cacheKey, out cached))
            {
                return cached;
            }

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IFraudRuleConfigurationRepository>();
            var config = await repository.GetByRuleNameAsync(ruleName, cancellationToken);

            if (config is not null)
            {
                _cache.Set(cacheKey, config, CacheDuration);
                _logger.LogDebug("Cached fraud rule configuration: {RuleName}", ruleName);
            }

            return config;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IEnumerable<FraudRuleConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(AllRulesCacheKey, out IEnumerable<FraudRuleConfiguration>? cached))
        {
            return cached!;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cache.TryGetValue(AllRulesCacheKey, out cached))
            {
                return cached!;
            }

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IFraudRuleConfigurationRepository>();
            var configs = (await repository.GetAllAsync(cancellationToken)).ToList();

            _cache.Set(AllRulesCacheKey, configs, CacheDuration);
            _logger.LogDebug("Cached all fraud rule configurations: {Count} rules", configs.Count);

            // Also cache individual rules
            foreach (var config in configs)
            {
                var individualCacheKey = $"{RuleCacheKeyPrefix}{config.RuleName}";
                _cache.Set(individualCacheKey, config, CacheDuration);
            }

            return configs;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Invalidate()
    {
        _cache.Remove(AllRulesCacheKey);
        _logger.LogInformation("Invalidated all fraud rule configuration cache");
    }

    public void Invalidate(string ruleName)
    {
        var cacheKey = $"{RuleCacheKeyPrefix}{ruleName}";
        _cache.Remove(cacheKey);
        _cache.Remove(AllRulesCacheKey);
        _logger.LogInformation("Invalidated fraud rule configuration cache for: {RuleName}", ruleName);
    }

    public async Task RefreshAsync(string ruleName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{RuleCacheKeyPrefix}{ruleName}";

        await _lock.WaitAsync(cancellationToken);
        try
        {
            // Remove existing cache entries
            _cache.Remove(cacheKey);
            _cache.Remove(AllRulesCacheKey);

            // Fetch fresh data from database
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IFraudRuleConfigurationRepository>();
            var config = await repository.GetByRuleNameAsync(ruleName, cancellationToken);

            // Push to cache
            if (config is not null)
            {
                _cache.Set(cacheKey, config, CacheDuration);
                _logger.LogInformation("Refreshed fraud rule configuration cache for: {RuleName}", ruleName);
            }
        }
        finally
        {
            _lock.Release();
        }
    }
}
