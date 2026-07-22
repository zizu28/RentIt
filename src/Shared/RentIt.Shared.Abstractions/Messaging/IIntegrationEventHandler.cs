namespace RentIt.Shared.Abstractions.Messaging;

/// <summary>
/// Handler interface for processing integration events.
/// Each consuming module implements this for the integration events it cares about.
/// </summary>
/// <typeparam name="TEvent">The integration event type to handle</typeparam>
public interface IIntegrationEventHandler<in TEvent> where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
