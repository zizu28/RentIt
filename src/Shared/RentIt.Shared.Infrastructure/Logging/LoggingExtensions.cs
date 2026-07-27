using Microsoft.AspNetCore.Builder;
using Serilog;

namespace RentIt.Shared.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSharedLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        return builder;
    }
}
