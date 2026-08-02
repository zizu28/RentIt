using MediatR;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Bookings.Infrastructure.Database;

internal sealed class DomainEventDispatcher(BookingsDbContext dbContext, IPublisher publisher)
{
    private readonly BookingsDbContext _dbContext = dbContext;
    private readonly IPublisher _publisher = publisher;

    public async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var aggregateRoots = _dbContext.ChangeTracker
            .Entries<IAggregateRoot<Guid>>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.Entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
