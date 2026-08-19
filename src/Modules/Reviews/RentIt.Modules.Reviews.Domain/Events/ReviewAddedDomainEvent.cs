using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Reviews.Domain.Events;

public record ReviewAddedDomainEvent(
    Guid ReviewId,
    Guid PropertyId,
    Guid GuestId,
    int Rating) : DomainEvent;
