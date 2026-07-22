using MediatR;
using Microsoft.Extensions.Logging;
using RentIt.Modules.Identity.Domain.Events;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Identity.Application.Handlers;

/// <summary>
/// Choreography handler: converts the UserRegisteredEvent domain event
/// into a UserRegisteredIntegrationEvent and publishes it to the event bus.
/// 
/// Consumers:
///   - Verification module → initiates email/phone verification
///   - Analytics module → tracks onboarding funnel
/// </summary>
public sealed class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<UserRegisteredDomainEventHandler> _logger;

    public UserRegisteredDomainEventHandler(IEventBus eventBus, ILogger<UserRegisteredDomainEventHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Saga:UserOnboarding] User {UserId} registered with email {Email}. Publishing integration event.",
            notification.UserId, notification.Email);

        var integrationEvent = new UserRegisteredIntegrationEvent(
            notification.UserId,
            notification.Email,
            notification.PhoneNumber,
            notification.Role);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation(
            "[Saga:UserOnboarding] UserRegisteredIntegrationEvent published for user {UserId}.",
            notification.UserId);
    }
}
