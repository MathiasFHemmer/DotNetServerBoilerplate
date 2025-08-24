using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MHemmer.Boilerplate.Api.Setup;

public static class TelemetrySetupExtensions
{
    public static WebApplicationBuilder SetupTelemetry(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resBuilder => resBuilder.AddService(serviceName: nameof(MHemmer.Boilerplate)))
            .WithLogging(logging =>
            {
                logging.AddConsoleExporter();
            })
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());
            return builder;
    }
}