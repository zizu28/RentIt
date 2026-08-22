using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;

namespace RentIt.Modules.Messaging.Infrastructure.Persistence.Repositories;

internal sealed class NotificationRepository(MessagingDbContext dbContext) : INotificationRepository
{
    private readonly MessagingDbContext _dbContext = dbContext;

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetUserUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
    }

    public void Update(Notification notification)
    {
        _dbContext.Notifications.Update(notification);
    }
}
