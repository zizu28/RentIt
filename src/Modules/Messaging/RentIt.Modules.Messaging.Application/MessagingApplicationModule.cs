using Microsoft.Extensions.DependencyInjection;

namespace RentIt.Modules.Messaging.Application;

public static class MessagingApplicationModule
{
    public static IServiceCollection AddMessagingApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(MessagingApplicationModule).Assembly);
        });

        return services;
    }
}
