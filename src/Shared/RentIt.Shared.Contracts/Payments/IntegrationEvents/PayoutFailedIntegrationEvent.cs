using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

/// <summary>
/// Published when a payout to a Host fails.
/// Consumed by: Bookings (trigger retry), Messaging (notify support)
/// </summary>
public sealed record PayoutFailedIntegrationEvent(
    Guid BookingId,
    Guid HostId,
    decimal AttemptedAmount,
    string Currency,
    string Reason
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
