using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Modules.Identity.Infrastructure.Messaging;

/// <summary>
/// In-memory event bus implementation for the modular monolith.
/// Dispatches integration events to all registered IIntegrationEventHandler implementations
/// within the same process. Can be replaced with RabbitMQ/MassTransit when migrating to microservices.
/// </summary>
internal sealed class InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger) : IEventBus
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<InMemoryEventBus> _logger = logger;

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) 
        where T : IIntegrationEvent
    {
        var eventType = @event.GetType().Name;
        _logger.LogInformation(
            "[EventBus] Publishing integration event {EventType} with Id {EventId}",
            eventType, @event.EventId);

        // Resolve all handlers for this integration event type
        var handlers = _serviceProvider.GetServices<IIntegrationEventHandler<T>>();

        foreach (var handler in handlers)
        {
            try
            {
                _logger.LogInformation(
                    "[EventBus] Dispatching {EventType} to {HandlerType}",
                    eventType, handler.GetType().Name);

                await handler.HandleAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[EventBus] Error handling {EventType} in {HandlerType}",
                    eventType, handler.GetType().Name);

                // In production, consider dead-letter queue or retry policy
                throw;
            }
        }
    }
}
