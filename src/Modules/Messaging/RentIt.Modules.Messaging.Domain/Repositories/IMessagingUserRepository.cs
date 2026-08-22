using RentIt.Modules.Messaging.Domain.Entities;

namespace RentIt.Modules.Messaging.Domain.Repositories;

public interface IMessagingUserRepository
{
    Task<MessagingUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(MessagingUser user, CancellationToken cancellationToken = default);
    void Update(MessagingUser user);
}
