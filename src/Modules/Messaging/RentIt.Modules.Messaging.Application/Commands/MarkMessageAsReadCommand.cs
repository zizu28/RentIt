using MediatR;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Messaging.Application.Commands;

public record MarkMessageAsReadCommand(Guid MessageId, Guid UserId) : IRequest<Result>;

internal sealed class MarkMessageAsReadCommandHandler(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<MarkMessageAsReadCommand, Result>
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
        if (message is null)
        {
            return Result.Failure(new Error("Message.NotFound", "Message not found."));
        }

        var conversation = await _conversationRepository.GetByIdAsync(message.ConversationId, cancellationToken);
        if (conversation is null || (conversation.Participant1Id != request.UserId && conversation.Participant2Id != request.UserId))
        {
            return Result.Failure(new Error("Message.AccessDenied", "Access denied."));
        }

        if (message.SenderId == request.UserId)
        {
            // You can't mark your own message as read by the recipient
            return Result.Success();
        }

        message.MarkAsRead(DateTimeOffset.UtcNow);
        _messageRepository.Update(message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
