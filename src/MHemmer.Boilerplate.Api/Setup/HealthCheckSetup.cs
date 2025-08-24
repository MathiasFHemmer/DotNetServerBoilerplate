using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MHemmer.Boilerplate.Api.Setup;

/// <summary>
/// Defines a "Liveness" status for the application
/// The application is live when it successfully launches its process, but not necessarily ready to answer requests.
/// To check if requests are ready to be accepted see <see85699999999999999999999999999999999 cref="ReadinessHealthCheck" /> 
/// </summary>
public class LivenessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}

/// <summary>
/// Defines a "Readiness" status for the application
/// The application is ready when its fully capable of answering users requests.
/// </summary>
public class ReadinessHealthCheck : IHealthCheck
{
    private volatile bool isReady = true;
    public bool IsReady { get => isReady; set => isReady = value; }
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (IsReady)
            return Task.FromResult(HealthCheckResult.Healthy());

        return Task.FromResult(HealthCheckResult.Unhealthy());
    }
}

/// <summary>
/// Checks if the application is able to reach the database provided.
/// The application is ready when its fully capable of answering users requests.
/// </summary>
public class DbConnectionHealthCheck<TContext> : IHealthCheck where TContext : DbContext
{
    private readonly ILogger<DbConnectionHealthCheck<TContext>> _logger;
    private readonly TContext _context;
    public DbConnectionHealthCheck(TContext context, ILogger<DbConnectionHealthCheck<TContext>> logger)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            return canConnect switch
            {
                true => HealthCheckResult.Healthy(),
                _ => HealthCheckResult.Unhealthy(),
            };
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unable to reach database!");
            return HealthCheckResult.Unhealthy();
        }
    }
}

/// <summary>
/// Maps the health check services to endpoints, enabling external tools to request for health status
/// "/health" will query for all health checks available in a single call and return a json object for each of them.
/// "/health/live" will query for the general application Liveness status and return a single text response.
/// "/health/ready" will query for the general application Readiness status and return a single text response.
/// "/health/ready/<service>" can be used as extensions for implementing service specific Readiness
/// </summary>
public static class HealthCheckSetupExtentions
{
    public static WebApplicationBuilder SetupHealthCheck<TDbContext>(this WebApplicationBuilder builder) where TDbContext : DbContext
    {
        var liveness = new LivenessHealthCheck();
        var readiness = new ReadinessHealthCheck();

        builder.Services.AddSingleton(readiness);
        builder.Services.AddSingleton(liveness);
        builder.Services.AddTransient<DbConnectionHealthCheck<TDbContext>>();

        builder.Services.AddHealthChecks()
            .AddCheck<LivenessHealthCheck>("live")
            .AddCheck<ReadinessHealthCheck>("ready")
            .AddCheck<DbConnectionHealthCheck<TDbContext>>("database");

        return builder;
    }

    public static WebApplication MapHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status,
                    checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString() })
                });
                await context.Response.WriteAsync(result);
            }
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Name == "live"
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Name == "ready"
        });
                app.MapHealthChecks("/health/database", new HealthCheckOptions
        {
            Predicate = check => check.Name == "database"
        });
        return app;
    }
}