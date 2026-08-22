using MediatR;
using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Messaging.Application.Queries;

public record GetConversationMessagesQuery(Guid ConversationId, Guid UserId) : IRequest<Result<IReadOnlyList<MessageDto>>>;

internal sealed class GetConversationMessagesQueryHandler(
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository) 
    : IRequestHandler<GetConversationMessagesQuery, Result<IReadOnlyList<MessageDto>>>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly IMessageRepository _messageRepository = messageRepository;

    public async Task<Result<IReadOnlyList<MessageDto>>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        
        if (conversation is null || 
            (conversation.Participant1Id != request.UserId && conversation.Participant2Id != request.UserId))
        {
            return Result.Failure<IReadOnlyList<MessageDto>>(new Error("Conversation.NotFound", "Conversation not found or access denied."));
        }

        var messages = await _messageRepository.GetConversationMessagesAsync(request.ConversationId, cancellationToken);
        
        var dtos = messages.Select(m => new MessageDto(
            m.Id,
            m.ConversationId,
            m.SenderId,
            m.Content,
            m.SentAt,
            m.ReadAt)).ToList();

        return Result.Success((IReadOnlyList<MessageDto>)dtos);
    }
}
