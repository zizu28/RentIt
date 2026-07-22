using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Identity.IntegrationEvents;

/// <summary>
/// Published when a new user registers.
/// Consumed by: Verification (initiate email/phone verification), Analytics (onboarding funnel)
/// </summary>
public sealed record UserRegisteredIntegrationEvent(
    Guid UserId,
    string Email,
    string PhoneNumber,
    string Role
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
