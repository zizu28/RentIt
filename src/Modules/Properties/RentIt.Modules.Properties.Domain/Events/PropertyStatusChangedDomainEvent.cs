using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Properties.Domain.Events;

public sealed record PropertyStatusChangedDomainEvent(
    Guid PropertyId,
    PropertyStatus OldStatus,
    PropertyStatus NewStatus) : DomainEvent;
