using Microsoft.EntityFrameworkCore.Storage;
using RentIt.Modules.Payments.Domain.Repositories;
using RentIt.Shared.Infrastructure.Messaging;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Payments.Infrastructure.Database;

internal sealed class PaymentsUnitOfWork(PaymentsDbContext dbContext, DomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    private readonly PaymentsDbContext _dbContext = dbContext;
    private readonly DomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;
    private IDbContextTransaction? _transaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events BEFORE successful save so outbox messages are added to the SAME transaction.
        await _domainEventDispatcher.DispatchDomainEventsAsync(cancellationToken);

        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        // Enqueue the hangfire job to process outbox messages immediately
        Hangfire.BackgroundJob.Enqueue<IProcessOutboxMessagesJob<PaymentsDbContext>>(x => x.ProcessAsync(CancellationToken.None));

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
