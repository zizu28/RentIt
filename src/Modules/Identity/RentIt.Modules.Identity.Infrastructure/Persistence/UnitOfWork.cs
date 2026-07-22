using Microsoft.EntityFrameworkCore.Storage;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Identity.Infrastructure.Persistence;

/// <summary>
/// Unit of work that dispatches domain events after a successful save.
/// Domain events are collected from all tracked aggregate roots and
/// published through MediatR, enabling the choreography-based saga pattern.
/// </summary>
internal sealed class UnitOfWork(IdentityDbContext dbContext, DomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    private readonly IdentityDbContext _dbContext = dbContext;
    private readonly DomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;
    private IDbContextTransaction? _transaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        // Dispatch domain events AFTER successful save
        await _domainEventDispatcher.DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}

