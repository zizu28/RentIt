using RentIt.Modules.Messaging.Application.DTOs;

namespace RentIt.Modules.Messaging.Api.Hubs;

public interface IMessagingClient
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveNotification(NotificationDto notification);
}
