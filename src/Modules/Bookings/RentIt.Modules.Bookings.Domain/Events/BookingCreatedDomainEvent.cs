using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Bookings.Domain.Events;

public sealed record BookingCreatedDomainEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid GuestId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalPrice,
    string Currency) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
