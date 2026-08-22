using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Application.Services;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Bookings.IntegrationEvents;

namespace RentIt.Modules.Messaging.Application.EventHandlers;

internal sealed class BookingRequestedIntegrationEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IMessagingHubPublisher hubPublisher,
    IBackgroundJob backgroundJob) : INotificationHandler<BookingRequestedIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMessagingHubPublisher _hubPublisher = hubPublisher;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;

    public async Task Handle(BookingRequestedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var content = $"You have a new booking request for your property ({notification.PropertyId}).";
        var dbNotification = Notification.Create(notification.HostId, content, DateTimeOffset.UtcNow);
        
        await _notificationRepository.AddAsync(dbNotification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto(dbNotification.Id, dbNotification.Content, dbNotification.CreatedAt, dbNotification.IsRead);
        await _hubPublisher.PublishNotificationAsync(notification.HostId, dto, cancellationToken);

        _backgroundJob.Enqueue<IEmailService>(
            "default", email => email.SendEmailAsync(
                "host@example.com", "New Booking Request", content));
        //await _emailService.SendEmailAsync("host@example.com", "New Booking Request", content, cancellationToken);
    }
}
