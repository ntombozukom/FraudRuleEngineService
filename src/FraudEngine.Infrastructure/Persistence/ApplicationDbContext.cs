using Microsoft.EntityFrameworkCore;
using FraudEngine.Domain.Entities;

namespace FraudEngine.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FraudAlert> FraudAlerts => Set<FraudAlert>();
    public DbSet<FraudRuleConfiguration> FraudRuleConfigurations => Set<FraudRuleConfiguration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<FraudRuleConfiguration>().HasData(
            new FraudRuleConfiguration
            {
                Id = Guid.Parse("c7b9a4e2-8f31-4d6a-b5c8-2e9f0a1b3d4e"),
                RuleName = "HighValueTransaction",
                IsEnabled = true,
                Parameters = "{\"ThresholdAmount\": 50000}",
                Description = "Flags transactions exceeding a configurable amount threshold.",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new FraudRuleConfiguration
            {
                Id = Guid.Parse("e3d7f2a1-6c48-4b9e-a0d5-8f2c1e7b9a3d"),
                RuleName = "RapidSuccessiveTransactions",
                IsEnabled = true,
                Parameters = "{\"MaxTransactions\": 3, \"TimeWindowMinutes\": 5}",
                Description = "Flags accounts with multiple transactions within a short time window.",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new FraudRuleConfiguration
            {
                Id = Guid.Parse("f8a2c6e4-9b15-4d7f-8e3a-1c0d5b9f2a6e"),
                RuleName = "CrossBorderTransaction",
                IsEnabled = true,
                Parameters = "{\"HomeCountry\": \"ZA\"}",
                Description = "Flags transactions originating from outside the home country.",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new FraudRuleConfiguration
            {
                Id = Guid.Parse("b4e8d1f5-3a72-4c0e-9d6b-7f5a2c8e1b4d"),
                RuleName = "AfterHoursTransaction",
                IsEnabled = true,
                Parameters = "{\"StartHour\": 23, \"EndHour\": 5}",
                Description = "Flags transactions occurring during unusual hours (23:00-05:00).",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new FraudRuleConfiguration
            {
                Id = Guid.Parse("d9c3b7a6-2e84-4f1d-a5b0-6c9e3d1f8a2b"),
                RuleName = "UnusualCategory",
                IsEnabled = true,
                Parameters = "{\"MinHistoryCount\": 10, \"UnusualThresholdPercent\": 5}",
                Description = "Flags transactions in spending categories unusual for the account.",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}
