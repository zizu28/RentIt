using MediatR;
using Microsoft.EntityFrameworkCore;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Infrastructure.Persistence;

/// <summary>
/// Intercepts SaveChangesAsync to dispatch domain events via MediatR
/// after the database transaction completes successfully.
/// This is the core of the choreography pattern — domain events raised
/// by aggregates are dispatched to handlers that publish integration events.
/// </summary>
internal sealed class DomainEventDispatcher(IdentityDbContext dbContext, IPublisher publisher) : IUnitOfWorkEventDispatcher
{
    private readonly IdentityDbContext _dbContext = dbContext;
    private readonly IPublisher _publisher = publisher;

    public async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        // Collect all domain events from tracked aggregate roots
        var aggregateRoots = _dbContext.ChangeTracker
            .Entries<IAggregateRoot<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(ar => ar.DomainEvents)
            .ToList();

        // Clear events before dispatching to prevent re-entrancy issues
        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }

        // Dispatch each domain event through MediatR
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}

/// <summary>
/// Interface for the domain event dispatching service
/// </summary>
internal interface IUnitOfWorkEventDispatcher
{
    Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default);
}
