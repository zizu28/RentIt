using RentIt.Modules.Analytics.Infrastructure.Database;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Analytics.Infrastructure.Repositories;

internal sealed class AnalyticsUnitOfWork(AnalyticsDbContext dbContext) : IUnitOfWork
{
    private readonly AnalyticsDbContext _dbContext = dbContext;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_dbContext.Database.CurrentTransaction is not null)
        {
            await _dbContext.Database.CommitTransactionAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_dbContext.Database.CurrentTransaction is not null)
        {
            await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
        }
    }
}
