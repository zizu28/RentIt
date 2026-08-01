using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Infrastructure.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddSharedMessaging(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, InMemoryEventBus>();
        return services;
    }
}
