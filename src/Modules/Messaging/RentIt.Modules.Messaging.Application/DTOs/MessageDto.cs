namespace RentIt.Modules.Messaging.Application.DTOs;

public record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTimeOffset SentAt,
    DateTimeOffset? ReadAt);
