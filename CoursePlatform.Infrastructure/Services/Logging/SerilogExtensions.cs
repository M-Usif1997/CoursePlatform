using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace CoursePlatform.Infrastructure.Services.Logging;

/// <summary>
/// Wires Serilog into the host, reading sinks and levels from the Serilog
/// configuration section.
/// </summary>
public static class SerilogExtensions
{
    public static IHostBuilder AddSerilogLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();
        });

        return hostBuilder;
    }
}
