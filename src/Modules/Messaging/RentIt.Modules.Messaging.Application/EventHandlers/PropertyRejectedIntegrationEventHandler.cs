using MediatR;
using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Application.Services;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;
using RentIt.Shared.Contracts.Verification.IntegrationEvents;

namespace RentIt.Modules.Messaging.Application.EventHandlers;

internal sealed class PropertyRejectedIntegrationEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IMessagingHubPublisher hubPublisher,
    IBackgroundJob backgroundJob) : INotificationHandler<PropertyRejectedIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingHubPublisher _hubPublisher = hubPublisher;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;

    public async Task Handle(PropertyRejectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var content = $"Your property listing has been rejected. Reason: {notification.Reason}";
        var dbNotification = Notification.Create(notification.HostId, content, DateTimeOffset.UtcNow);
        
        await _notificationRepository.AddAsync(dbNotification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto(dbNotification.Id, dbNotification.Content, dbNotification.CreatedAt, dbNotification.IsRead);
        await _hubPublisher.PublishNotificationAsync(notification.HostId, dto, cancellationToken);

        // We assume we know the user's email, or we mock it for now.
        // In a real app we would fetch the user's email from the Identity module via an RPC call or cached read model.
        //await _emailService.SendEmailAsync("host@example.com", "Property Rejected", content, cancellationToken);
        _backgroundJob.Enqueue<IEmailService>(
            "default", email => email.SendEmailAsync(
                "host@example.com", "Property Rejected", content));
    }
}
