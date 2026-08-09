using MediatR;
using Microsoft.Extensions.Logging;
using RentIt.Modules.Identity.Domain.Events;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class UserDeletedDomainEventHandler : INotificationHandler<UserDeletedEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<UserDeletedDomainEventHandler> _logger;

    public UserDeletedDomainEventHandler(IEventBus eventBus, ILogger<UserDeletedDomainEventHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Saga:UserDeletion] User {UserId} deleted. Publishing integration event.",
            notification.UserId);

        var integrationEvent = new UserDeletedIntegrationEvent(
            notification.UserId,
            notification.Role);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation(
            "[Saga:UserDeletion] UserDeletedIntegrationEvent published for user {UserId}.",
            notification.UserId);
    }
}
