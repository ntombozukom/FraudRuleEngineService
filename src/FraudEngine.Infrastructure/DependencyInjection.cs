using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FraudEngine.Application.Services;
using FraudEngine.Domain.Interfaces;
using FraudEngine.Infrastructure.Caching;
using FraudEngine.Infrastructure.FraudRules;
using FraudEngine.Infrastructure.Persistence;
using FraudEngine.Infrastructure.Persistence.Repositories;

namespace FraudEngine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Register memory cache
        services.AddMemoryCache();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();
        services.AddScoped<IFraudRuleConfigurationRepository, FraudRuleConfigurationRepository>();

        // Register fraud rule configuration cache (Singleton for shared cache across requests)
        services.AddSingleton<IFraudRuleConfigurationCache, FraudRuleConfigurationCache>();

        // Register fraud rules (Strategy Pattern)
        services.AddScoped<IFraudRule, HighValueTransactionRule>();
        services.AddScoped<IFraudRule, RapidSuccessiveTransactionsRule>();
        services.AddScoped<IFraudRule, CrossBorderTransactionRule>();
        services.AddScoped<IFraudRule, AfterHoursTransactionRule>();
        services.AddScoped<IFraudRule, UnusualCategoryRule>();

        // Register fraud rule engine
        services.AddScoped<IFraudRuleEngine, FraudEngine.Infrastructure.Services.FraudRuleEngine>();

        return services;
    }
}
