using MediatR;
using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Messaging.Application.Queries;

public record GetConversationsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<ConversationDto>>>;

internal sealed class GetConversationsQueryHandler(IConversationRepository conversationRepository) 
    : IRequestHandler<GetConversationsQuery, Result<IReadOnlyList<ConversationDto>>>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;

    public async Task<Result<IReadOnlyList<ConversationDto>>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _conversationRepository.GetUserConversationsAsync(request.UserId, cancellationToken);
        
        var dtos = conversations.Select(c => new ConversationDto(
            c.Id,
            c.Participant1Id,
            c.Participant2Id,
            c.LastMessageAt)).ToList();

        return Result.Success((IReadOnlyList<ConversationDto>)dtos);
    }
}
