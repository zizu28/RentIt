using Microsoft.EntityFrameworkCore.Storage;
using RentIt.Modules.Payments.Domain.Repositories;

namespace RentIt.Modules.Payments.Infrastructure.Database;

internal sealed class PaymentsUnitOfWork(PaymentsDbContext dbContext, DomainEventDispatcher domainEventDispatcher) : IPaymentsUnitOfWork
{
    private readonly PaymentsDbContext _dbContext = dbContext;
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
