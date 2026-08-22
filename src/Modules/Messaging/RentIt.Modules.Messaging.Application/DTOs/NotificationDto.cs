namespace RentIt.Modules.Messaging.Application.DTOs;

public record NotificationDto(
    Guid Id,
    string Content,
    DateTimeOffset CreatedAt,
    bool IsRead);
