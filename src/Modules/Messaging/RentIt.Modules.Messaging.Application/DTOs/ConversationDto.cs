namespace RentIt.Modules.Messaging.Application.DTOs;

public record ConversationDto(
    Guid Id,
    Guid Participant1Id,
    Guid Participant2Id,
    DateTimeOffset LastMessageAt);
