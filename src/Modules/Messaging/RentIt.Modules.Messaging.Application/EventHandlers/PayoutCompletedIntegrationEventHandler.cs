using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Application.Services;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Messaging.Application.EventHandlers;

internal sealed class PayoutCompletedIntegrationEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IMessagingHubPublisher hubPublisher,
    IBackgroundJob backgroundJob) : INotificationHandler<PayoutCompletedIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailService _emailService = emailService;
    private readonly IMessagingHubPublisher _hubPublisher = hubPublisher;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;

    public async Task Handle(PayoutCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var content = $"Your payout of {notification.NetAmount} {notification.Currency} for booking {notification.BookingId} has been completed.";
        var dbNotification = Notification.Create(notification.HostId, content, DateTimeOffset.UtcNow);
        
        await _notificationRepository.AddAsync(dbNotification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto(dbNotification.Id, dbNotification.Content, dbNotification.CreatedAt, dbNotification.IsRead);
        await _hubPublisher.PublishNotificationAsync(notification.HostId, dto, cancellationToken);

        _backgroundJob.Enqueue<IEmailService>(
           "default", email => email.SendEmailAsync(
               "host@example.com", "Payout Completed", content));

        //await _emailService.SendEmailAsync("host@example.com", "Payout Completed", content, cancellationToken);
    }
}
