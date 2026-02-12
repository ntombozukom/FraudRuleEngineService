using Microsoft.EntityFrameworkCore;
using Serilog;
using FraudEngine.API.Constants;
using FraudEngine.API.Endpoints.AuditLogs;
using FraudEngine.API.Endpoints.FraudAlerts;
using FraudEngine.API.Endpoints.FraudRules;
using FraudEngine.API.Endpoints.Transactions;
using FraudEngine.API.Middleware;
using FraudEngine.Infrastructure.Persistence;

namespace FraudEngine.API.Extensions;

public static class ApplicationExtensions
{
    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fraud Rule Engine API v1");
            c.RoutePrefix = string.Empty;
        });

        return app;
    }

    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Only use HTTPS redirection when not in Docker/container environment
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")) &&
            Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Contains("https"))
        {
            app.UseHttpsRedirection();
        }

        return app;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapEvaluateTransaction();

        app.MapGetFraudAlerts();
        app.MapGetFraudAlertByReference();
        app.MapReviewFraudAlert();
        app.MapGetFraudAlertStatistics();

        app.MapGetFraudRules();
        app.MapUpdateFraudRule();

        app.MapGetAuditLogs();

        return app;
    }

    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (db.Database.IsRelational())
        {
            await ApplyRelationalMigrationsAsync(db, logger, config);
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }
    }

    private static async Task ApplyRelationalMigrationsAsync(ApplicationDbContext db, Microsoft.Extensions.Logging.ILogger logger, IConfiguration config)
    {
        var maxRetries = config.GetValue(ConfigurationKeys.DatabaseStartup.MaxRetries, ConfigurationDefaults.MaxRetries);
        var retryDelaySeconds = config.GetValue(ConfigurationKeys.DatabaseStartup.RetryDelaySeconds, ConfigurationDefaults.RetryDelaySeconds);
        var delay = TimeSpan.FromSeconds(retryDelaySeconds);

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                logger.LogInformation(
                    "Trying database connection (attempt {Attempt}/{MaxRetries})...",
                    attempt, maxRetries);

                await db.Database.MigrateAsync();

                logger.LogInformation("Database migration completed successfully");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                logger.LogWarning(
                    "Database not ready, waiting {Delay}s... ({Message})",
                    delay.TotalSeconds, ex.Message);

                await Task.Delay(delay);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to connect to database after {MaxRetries} attempts. Please ensure Docker Desktop is running and execute: docker-compose up -d",
                    maxRetries);

                throw new InvalidOperationException($"Database connection failed after {maxRetries} attempts. " +
                    "Ensure Docker Desktop is running and SQL Server container is started. " +
                    "Run 'docker-compose up -d' to start the database.", ex);
            }
        }
    }
}
