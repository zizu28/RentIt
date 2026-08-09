using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Identity.IntegrationEvents;

public sealed record UserDeletedIntegrationEvent(
    Guid UserId,
    string Role
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
