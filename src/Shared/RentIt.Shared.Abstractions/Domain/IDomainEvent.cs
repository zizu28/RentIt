namespace RentIt.Shared.Abstractions.Domain;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
