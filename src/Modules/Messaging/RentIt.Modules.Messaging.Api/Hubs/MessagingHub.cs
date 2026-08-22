using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RentIt.Modules.Messaging.Api.Hubs;

[Authorize]
public class MessagingHub(ILogger<MessagingHub> logger) : Hub<IMessagingClient>
{
    private readonly ILogger<MessagingHub> _logger = logger;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("User {UserId} connected to MessagingHub with connection ID {ConnectionId}", userId, Context.ConnectionId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation(exception, "User {UserId} disconnected from MessagingHub with connection ID {ConnectionId}", userId, Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }
}
