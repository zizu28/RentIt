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
public class MessagesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var query = new GetConversationsQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("conversations/{conversationId:guid}")]
    public async Task<IActionResult> GetMessages(Guid conversationId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var query = new GetConversationMessagesQuery(conversationId, userId);
        var result = await _mediator.Send(query, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(SendMessageRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var command = new SendMessageCommand(userId, request.RecipientId, request.Content);
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{messageId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid messageId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var command = new MarkMessageAsReadCommand(messageId, userId);
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}

public record SendMessageRequest(Guid RecipientId, string Content);
