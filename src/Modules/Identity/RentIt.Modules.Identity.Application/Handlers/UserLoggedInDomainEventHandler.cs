using MediatR;
using Microsoft.Extensions.Logging;
using RentIt.Modules.Identity.Domain.Events;
using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Modules.Identity.Application.Handlers;

/// <summary>
/// Choreography handler: processes the UserLoggedInEvent domain event.
/// 
/// In the choreography pattern, this handler can publish integration events
/// to notify other modules of user activity (e.g., Analytics for login tracking,
/// Messaging for online status updates).
/// </summary>
public sealed class UserLoggedInDomainEventHandler : INotificationHandler<UserLoggedInEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<UserLoggedInDomainEventHandler> _logger;

    public UserLoggedInDomainEventHandler(IEventBus eventBus, ILogger<UserLoggedInDomainEventHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Identity] User {UserId} ({Email}) logged in at {LoginAt}.",
            notification.UserId, notification.Email, notification.LoginAt);

        // Login events are typically consumed by Analytics for tracking.
        // No integration event needed yet — add one when Analytics module is built:
        // await _eventBus.PublishAsync(new UserLoggedInIntegrationEvent(...), cancellationToken);

        await Task.CompletedTask;
    }
}
