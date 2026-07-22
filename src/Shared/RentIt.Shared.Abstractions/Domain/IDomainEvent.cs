using MediatR;

namespace RentIt.Shared.Abstractions.Domain;

/// <summary>
/// Base interface for domain events.
/// Extends INotification to enable dispatching through MediatR's notification pipeline.
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
