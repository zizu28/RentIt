using Microsoft.AspNetCore.SignalR;
using RentIt.Modules.Messaging.Api.Hubs;
using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Application.Services;

namespace RentIt.Modules.Messaging.Api.Services;

internal sealed class MessagingHubPublisher(IHubContext<MessagingHub, IMessagingClient> hubContext) : IMessagingHubPublisher
{
    private readonly IHubContext<MessagingHub, IMessagingClient> _hubContext = hubContext;

    public async Task PublishMessageAsync(Guid recipientId, MessageDto message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
        .User(recipientId.ToString())
        .ReceiveMessage(message);
    }

    public async Task PublishNotificationAsync(Guid recipientId, NotificationDto notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
        .User(recipientId.ToString())
        .ReceiveNotification(notification);
    }
}
