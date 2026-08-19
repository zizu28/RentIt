using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Verification.Infrastructure.Database;

namespace RentIt.Modules.Verification.Infrastructure.Database;

internal sealed class VerificationUnitOfWork(VerificationDbContext dbContext) : IUnitOfWork
{
    private readonly VerificationDbContext _dbContext = dbContext;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            await _dbContext.Database.CurrentTransaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            await _dbContext.Database.CurrentTransaction.RollbackAsync(cancellationToken);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
