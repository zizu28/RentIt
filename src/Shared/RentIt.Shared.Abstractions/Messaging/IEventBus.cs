namespace RentIt.Shared.Abstractions.Messaging;

/// <summary>
/// Event bus for publishing integration events
/// </summary>
public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}
