using MediatR;
using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Application.Services;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Exceptions;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Messaging.Application.Commands;

public record SendMessageCommand(Guid SenderId, Guid RecipientId, string Content) : IRequest<Result<MessageDto>>;

internal sealed class SendMessageCommandHandler(
    IMessagingUserRepository userRepository,
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository,
    IUnitOfWork unitOfWork,
    IMessagingHubPublisher hubPublisher) : IRequestHandler<SendMessageCommand, Result<MessageDto>>
{
    private readonly IMessagingUserRepository _userRepository = userRepository;
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingHubPublisher _hubPublisher = hubPublisher;

    public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _userRepository.GetByIdAsync(request.SenderId, cancellationToken);
        if (sender is not null && sender.IsSuspended)
        {
            return Result.Failure<MessageDto>(new Error("User.Suspended", "Suspended users cannot send messages."));
        }

        var conversation = await _conversationRepository.GetByParticipantsAsync(request.SenderId, request.RecipientId, cancellationToken);
        
        if (conversation is null)
        {
            conversation = Conversation.Create(request.SenderId, request.RecipientId, DateTimeOffset.UtcNow);
            await _conversationRepository.AddAsync(conversation, cancellationToken);
        }

        var message = Message.Create(conversation.Id, request.SenderId, request.Content, DateTimeOffset.UtcNow);
        await _messageRepository.AddAsync(message, cancellationToken);

        conversation.UpdateLastMessageAt(message.SentAt);
        _conversationRepository.Update(conversation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var messageDto = new MessageDto(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.Content,
            message.SentAt,
            message.ReadAt);

        // Publish to recipient via SignalR
        await _hubPublisher.PublishMessageAsync(request.RecipientId, messageDto, cancellationToken);

        return Result.Success(messageDto);
    }
}
