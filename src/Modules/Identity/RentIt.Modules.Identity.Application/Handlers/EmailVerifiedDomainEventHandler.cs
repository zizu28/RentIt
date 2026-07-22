using MediatR;
using Microsoft.Extensions.Logging;
using RentIt.Modules.Identity.Domain.Events;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Identity.Application.Handlers;

/// <summary>
/// Choreography handler: converts the EmailVerifiedEvent domain event
/// into a log entry and checks if the user is now fully verified.
/// 
/// In a full implementation, this would query the user to check if both
/// email and phone are verified, and if so, publish a UserFullyVerifiedIntegrationEvent.
/// For now, it logs the verification step in the onboarding saga.
/// </summary>
public sealed class EmailVerifiedDomainEventHandler(IEventBus eventBus, ILogger<EmailVerifiedDomainEventHandler> logger) : INotificationHandler<EmailVerifiedEvent>
{
    private readonly IEventBus _eventBus = eventBus;
    private readonly ILogger<EmailVerifiedDomainEventHandler> _logger = logger;

    public async Task Handle(EmailVerifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Saga:UserOnboarding] Email verified for user {UserId} ({Email}). Checking full verification status.",
            notification.UserId, notification.Email);

        // Note: In a complete implementation, you would query the user repository
        // to check if both email AND phone are verified, then publish:
        // await _eventBus.PublishAsync(new UserFullyVerifiedIntegrationEvent(...), cancellationToken);
        // 
        // This is left for when the Verification module is built out, since
        // the Verification module owns the orchestration of verification steps.

        await Task.CompletedTask;
    }
}
