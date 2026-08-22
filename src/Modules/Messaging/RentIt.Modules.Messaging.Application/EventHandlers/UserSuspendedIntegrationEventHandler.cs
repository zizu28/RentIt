using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Messaging.Application.EventHandlers;

internal sealed class UserSuspendedIntegrationEventHandler(
    IMessagingUserRepository userRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<UserSuspendedIntegrationEvent>
{
    private readonly IMessagingUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UserSuspendedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);
        if (user is null)
        {
            user = MessagingUser.Create(notification.UserId);
            user.Suspend();
            await _userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            user.Suspend();
            _userRepository.Update(user);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
