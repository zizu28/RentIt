using MediatR;
using RentIt.Modules.Messaging.Application.DTOs;
using RentIt.Modules.Messaging.Application.Services;
using RentIt.Modules.Messaging.Domain.Entities;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Reviews.IntegrationEvents;

namespace RentIt.Modules.Messaging.Application.EventHandlers;

internal sealed class ReviewPublishedIntegrationEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IMessagingHubPublisher hubPublisher) : INotificationHandler<ReviewPublishedIntegrationEvent>
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailService _emailService = emailService;
    private readonly IMessagingHubPublisher _hubPublisher = hubPublisher;

    public async Task Handle(ReviewPublishedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var content = $"You have received a new {notification.Rating}-star review for your property ({notification.PropertyId}).";
        var dbNotification = Notification.Create(notification.HostId, content, DateTimeOffset.UtcNow);
        
        await _notificationRepository.AddAsync(dbNotification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto(dbNotification.Id, dbNotification.Content, dbNotification.CreatedAt, dbNotification.IsRead);
        await _hubPublisher.PublishNotificationAsync(notification.HostId, dto, cancellationToken);

        await _emailService.SendEmailAsync("host@example.com", "New Review Received", content, cancellationToken);
    }
}
