using MediatR;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Messaging.Application.Commands;

public record MarkNotificationAsReadCommand(Guid NotificationId, Guid UserId) : IRequest<Result>;

internal sealed class MarkNotificationAsReadCommandHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        
        if (notification is null || notification.UserId != request.UserId)
        {
            return Result.Failure(new Error("Notification.NotFound", "Notification not found or access denied."));
        }

        notification.MarkAsRead();
        _notificationRepository.Update(notification);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
