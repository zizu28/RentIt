using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Messaging.Domain.Entities;

public class Message : Entity<Guid>
{
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Content { get; private set; }
    public DateTimeOffset SentAt { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

#pragma warning disable CS8618
    private Message() { }
#pragma warning restore CS8618

    private Message(Guid id, Guid conversationId, Guid senderId, string content, DateTimeOffset sentAt)
    {
        Id = id;
        ConversationId = conversationId;
        SenderId = senderId;
        Content = content;
        SentAt = sentAt;
    }

    public static Message Create(Guid conversationId, Guid senderId, string content, DateTimeOffset sentAt)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content cannot be empty.", nameof(content));
        }

        return new Message(Guid.NewGuid(), conversationId, senderId, content, sentAt);
    }

    public void MarkAsRead(DateTimeOffset readAt)
    {
        if (ReadAt is null)
        {
            ReadAt = readAt;
        }
    }
}
