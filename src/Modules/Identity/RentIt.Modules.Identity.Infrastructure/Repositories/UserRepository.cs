using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Identity.Domain.Entities;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Modules.Identity.Infrastructure.Persistence;

namespace RentIt.Modules.Identity.Infrastructure.Repositories;

internal sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    private readonly IdentityDbContext _dbContext = dbContext;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .FromSqlRaw("SELECT TOP(1) * FROM [identity].[Users] WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", id)
            .ToListAsync(cancellationToken);
            
        return users.FirstOrDefault();
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<User?> GetByEmailForUpdateAsync(string email, CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .FromSqlRaw("SELECT TOP(1) * FROM [identity].[Users] WITH (UPDLOCK, ROWLOCK) WHERE Email = {0}", email)
            .ToListAsync(cancellationToken);
            
        return users.FirstOrDefault();
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber.Value == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.PhoneNumber.Value == phoneNumber, cancellationToken);
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(entity, cancellationToken);
    }

    public void Update(User entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(entity);
    }

    public void Delete(User entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Remove(entity);
    }
}
