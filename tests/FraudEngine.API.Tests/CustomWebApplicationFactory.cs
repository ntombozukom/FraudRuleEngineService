using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FraudEngine.Domain.Entities;
using FraudEngine.Infrastructure.Persistence;

namespace FraudEngine.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                         || d.ServiceType == typeof(DbContextOptions))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            var dbName = "TestDb_" + Guid.NewGuid();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            if (!db.FraudRuleConfigurations.Any())
            {
                var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                db.FraudRuleConfigurations.AddRange(
                    new FraudRuleConfiguration
                    {
                        Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                        RuleName = "HighValueTransaction",
                        IsEnabled = true,
                        Parameters = "{\"ThresholdAmount\": 50000}",
                        Description = "Flags transactions exceeding a configurable amount threshold.",
                        CreatedAt = seedDate,
                        UpdatedAt = seedDate
                    },
                    new FraudRuleConfiguration
                    {
                        Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                        RuleName = "RapidSuccessiveTransactions",
                        IsEnabled = true,
                        Parameters = "{\"MaxTransactions\": 3, \"TimeWindowMinutes\": 5}",
                        Description = "Flags accounts with multiple transactions within a short time window.",
                        CreatedAt = seedDate,
                        UpdatedAt = seedDate
                    },
                    new FraudRuleConfiguration
                    {
                        Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                        RuleName = "CrossBorderTransaction",
                        IsEnabled = true,
                        Parameters = "{\"HomeCountry\": \"ZA\"}",
                        Description = "Flags transactions originating from outside the home country.",
                        CreatedAt = seedDate,
                        UpdatedAt = seedDate
                    },
                    new FraudRuleConfiguration
                    {
                        Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                        RuleName = "AfterHoursTransaction",
                        IsEnabled = true,
                        Parameters = "{\"StartHour\": 23, \"EndHour\": 5}",
                        Description = "Flags transactions occurring during unusual hours (23:00-05:00).",
                        CreatedAt = seedDate,
                        UpdatedAt = seedDate
                    },
                    new FraudRuleConfiguration
                    {
                        Id = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                        RuleName = "UnusualCategory",
                        IsEnabled = true,
                        Parameters = "{\"MinHistoryCount\": 10, \"UnusualThresholdPercent\": 5}",
                        Description = "Flags transactions in spending categories unusual for the account.",
                        CreatedAt = seedDate,
                        UpdatedAt = seedDate
                    }
                );

                db.SaveChanges();
            }
        });
    }
}
