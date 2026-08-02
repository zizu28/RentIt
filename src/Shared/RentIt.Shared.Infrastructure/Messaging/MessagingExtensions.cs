using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Infrastructure.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddSharedMessaging(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, InMemoryEventBus>();
        
        // Register open generics for Outbox and Inbox per DbContext
        services.AddScoped(typeof(IOutboxService<>), typeof(OutboxService<>));
        services.AddScoped(typeof(IInboxService<>), typeof(InboxService<>));
        services.AddScoped(typeof(IProcessOutboxMessagesJob<>), typeof(ProcessOutboxMessagesJob<>));
        
        return services;
    }
}
