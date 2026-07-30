using Microsoft.EntityFrameworkCore.Storage;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Modules.Properties.Infrastructure.Database;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Properties.Infrastructure.Repositories;

internal sealed class PropertiesUnitOfWork(PropertiesDbContext dbContext) : IPropertiesUnitOfWork
{
    private readonly PropertiesDbContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null) return;
        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) return;
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null) return;
        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
