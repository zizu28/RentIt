using Microsoft.EntityFrameworkCore.Storage;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Infrastructure.Messaging;
using Hangfire;

namespace RentIt.Modules.Bookings.Infrastructure.Database;

internal sealed class BookingsUnitOfWork(
    BookingsDbContext dbContext,
    DomainEventDispatcher domainEventDispatcher,
    Hangfire.IBackgroundJobClient backgroundJobClient) : IUnitOfWork
{
    private readonly BookingsDbContext _dbContext = dbContext;
    private readonly DomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;
    private readonly Hangfire.IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private IDbContextTransaction? _transaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _domainEventDispatcher.DispatchDomainEventsAsync(cancellationToken);

        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        _backgroundJobClient.Enqueue<IProcessOutboxMessagesJob<BookingsDbContext>>(x => x.ProcessAsync(CancellationToken.None));

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
