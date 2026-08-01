using MediatR;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Infrastructure.Messaging;

internal sealed class InMemoryEventBus(IPublisher publisher) : IEventBus
{
    private readonly IPublisher _publisher = publisher;

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        await _publisher.Publish(@event, cancellationToken);
    }
}
