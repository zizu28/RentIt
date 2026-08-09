using MediatR;
using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Contracts.Identity.IntegrationEvents;

namespace RentIt.Modules.Properties.Application.EventHandlers;

internal sealed class UserDeletedIntegrationEventHandler(
    IPropertyRepository propertyRepository
) : INotificationHandler<UserDeletedIntegrationEvent>
{
    public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Role == "Host")
        {
            var properties = await propertyRepository.GetByHostIdAsync(notification.UserId, cancellationToken);
            foreach (var property in properties)
            {
                if (property.Status != PropertyStatus.Unlisted)
                {
                    property.ChangeStatus(PropertyStatus.Unlisted);
                    await propertyRepository.UpdateAsync(property, cancellationToken);
                }
            }
        }
    }
}
