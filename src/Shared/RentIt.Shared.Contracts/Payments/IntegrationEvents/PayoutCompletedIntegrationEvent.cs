using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

/// <summary>
/// Published when a payout to a landlord is successfully completed.
/// Consumed by: Bookings (mark booking as fully settled), Messaging (notify landlord)
/// </summary>
public sealed record PayoutCompletedIntegrationEvent(
    Guid PayoutId,
    Guid BookingId,
    Guid LandlordId,
    decimal NetAmount,
    decimal PlatformFee,
    string Currency
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
