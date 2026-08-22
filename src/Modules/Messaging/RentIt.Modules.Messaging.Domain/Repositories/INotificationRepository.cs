using RentIt.Modules.Messaging.Domain.Entities;

namespace RentIt.Modules.Messaging.Domain.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetUserUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    void Update(Notification notification);
}
