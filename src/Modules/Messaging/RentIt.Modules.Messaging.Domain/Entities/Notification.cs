using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Messaging.Domain.Entities;

public class Notification : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Content { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsRead { get; private set; }

#pragma warning disable CS8618
    private Notification() { }
#pragma warning restore CS8618

    private Notification(Guid id, Guid userId, string content, DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        Content = content;
        CreatedAt = createdAt;
        IsRead = false;
    }

    public static Notification Create(Guid userId, string content, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Notification content cannot be empty.", nameof(content));
        }

        return new Notification(Guid.NewGuid(), userId, content, createdAt);
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
