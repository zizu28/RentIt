using MediatR;

namespace RentIt.Shared.Abstractions.Messaging;

/// <summary>
/// Base interface for integration events
/// </summary>
public interface IIntegrationEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
