using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentIt.Modules.Messaging.Application.Commands;
using RentIt.Modules.Messaging.Application.Queries;
using System.Security.Claims;

namespace RentIt.Modules.Messaging.Api.Controllers;

[ApiController]
[Route("api/messaging/[controller]")]
[Authorize]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var query = new GetUnreadNotificationsQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var command = new MarkNotificationAsReadCommand(notificationId, userId);
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
