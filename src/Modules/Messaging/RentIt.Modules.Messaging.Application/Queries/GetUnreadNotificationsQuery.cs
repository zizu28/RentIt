using MediatR;
using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Messaging.Application.Queries;

public record GetUnreadNotificationsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<NotificationDto>>>;

internal sealed class GetUnreadNotificationsQueryHandler(INotificationRepository notificationRepository) 
    : IRequestHandler<GetUnreadNotificationsQuery, Result<IReadOnlyList<NotificationDto>>>
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;

    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetUserUnreadNotificationsAsync(request.UserId, cancellationToken);
        
        var dtos = notifications.Select(n => new NotificationDto(
            n.Id,
            n.Content,
            n.CreatedAt,
            n.IsRead)).ToList();

        return Result.Success((IReadOnlyList<NotificationDto>)dtos);
    }
}
