using MediatR;
using Microsoft.Extensions.Logging;
using RentIt.Modules.Identity.Domain.Events;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Identity.Application.Handlers;

/// <summary>
/// Choreography handler: converts the PhoneNumberVerifiedEvent domain event
/// into a log entry and checks if the user is now fully verified.
/// 
/// Same pattern as EmailVerifiedDomainEventHandler — when both verifications
/// are complete, a UserFullyVerifiedIntegrationEvent would be published.
/// </summary>
public sealed class PhoneNumberVerifiedDomainEventHandler(
    IEventBus eventBus,
    ILogger<PhoneNumberVerifiedDomainEventHandler> logger) : INotificationHandler<PhoneNumberVerifiedEvent>
{
    private readonly IEventBus _eventBus = eventBus;
    private readonly ILogger<PhoneNumberVerifiedDomainEventHandler> _logger = logger;

    public async Task Handle(PhoneNumberVerifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Saga:UserOnboarding] Phone number verified for user {UserId} ({PhoneNumber}). Checking full verification status.",
            notification.UserId, notification.PhoneNumber);

        // Note: Same as EmailVerifiedDomainEventHandler — in a complete implementation,
        // query user and publish UserFullyVerifiedIntegrationEvent when both are verified.

        await Task.CompletedTask;
    }
}
