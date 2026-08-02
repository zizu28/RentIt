using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Properties.IntegrationEvents;

public sealed record PropertyUnpublishedIntegrationEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid PropertyId
) : IIntegrationEvent;
