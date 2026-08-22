using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;

namespace RentIt.Modules.Messaging.Infrastructure.Persistence.Repositories;

internal sealed class MessagingUserRepository(MessagingDbContext dbContext) : IMessagingUserRepository
{
    private readonly MessagingDbContext _dbContext = dbContext;

    public async Task<MessagingUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(MessagingUser user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Update(MessagingUser user)
    {
        _dbContext.Users.Update(user);
    }
}
