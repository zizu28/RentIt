using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Messaging.Api.Hubs;
using RentIt.Modules.Messaging.Api.Services;
using RentIt.Modules.Messaging.Application;
using RentIt.Modules.Messaging.Application.Services;
using RentIt.Modules.Messaging.Infrastructure;

namespace RentIt.Modules.Messaging.Api;

public static class MessagingModule
{
    public static IServiceCollection AddMessagingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessagingApplication();
        services.AddMessagingInfrastructure(configuration);

        services.AddSignalR();
        services.AddTransient<IMessagingHubPublisher, MessagingHubPublisher>();

        return services;
    }

    public static IEndpointRouteBuilder MapMessagingModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<MessagingHub>("/hubs/messaging");
        
        return endpoints;
    }
}
