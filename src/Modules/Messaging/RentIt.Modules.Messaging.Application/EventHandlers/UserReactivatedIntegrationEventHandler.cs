using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Messaging.Application.EventHandlers;

internal sealed class UserReactivatedIntegrationEventHandler(
    IMessagingUserRepository userRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<UserReactivatedIntegrationEvent>
{
    private readonly IMessagingUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UserReactivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
        if (user is not null)
        {
            user.Reactivate();
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
