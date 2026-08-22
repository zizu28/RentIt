using RentIt.Modules.Messaging.Application.DTOs;

namespace RentIt.Modules.Messaging.Application.Services;

public interface IMessagingHubPublisher
{
    Task PublishMessageAsync(Guid recipientId, MessageDto message, CancellationToken cancellationToken = default);
    Task PublishNotificationAsync(Guid recipientId, NotificationDto notification, CancellationToken cancellationToken = default);
}
