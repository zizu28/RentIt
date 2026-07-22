using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Verification.IntegrationEvents;

/// <summary>
/// Published when a user completes all verification steps (email + phone).
/// Consumed by: Identity (update verification flags), Analytics (onboarding completion)
/// </summary>
public sealed record UserFullyVerifiedIntegrationEvent(
    Guid UserId,
    string Email,
    bool IsEmailVerified,
    bool IsPhoneVerified
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
