using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Shared.Abstractions.Domain;
using RentIt.Shared.Kernel.ValueObjects;

namespace RentIt.Modules.Properties.Domain.Events;

public sealed record PropertyPricingUpdatedDomainEvent(
    Guid PropertyId,
    RentalPeriod OldRentalPeriod,
    RentalPeriod NewRentalPeriod,
    Money OldPrice,
    Money NewPrice) : DomainEvent;
