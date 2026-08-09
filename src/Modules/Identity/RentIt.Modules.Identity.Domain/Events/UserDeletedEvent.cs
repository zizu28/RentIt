using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Domain.Events;

public sealed record UserDeletedEvent(Guid UserId, string Role) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
