using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Shared.Abstractions.Domain;
using RentIt.Shared.Kernel.ValueObjects;

namespace RentIt.Modules.Properties.Domain.Events;

public sealed record PropertyCreatedDomainEvent(
    Guid PropertyId,
    Guid HostId,
    string Name,
    PropertyType Type,
    RentalPeriod RentalPeriod,
    Money PricePerPeriod) : DomainEvent;
