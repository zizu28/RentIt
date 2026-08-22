using RentIt.Shared.Abstractions.Domain;
using RentIt.Modules.Messaging.Domain.Entities;

namespace RentIt.Modules.Messaging.Domain.Entities;

public class Conversation : Entity<Guid>
{
    public Guid Participant1Id { get; private set; }
    public Guid Participant2Id { get; private set; }
    public DateTimeOffset LastMessageAt { get; private set; }

    private readonly List<Message> _messages = [];
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

#pragma warning disable CS8618
    private Conversation() { }
#pragma warning restore CS8618

    private Conversation(Guid id, Guid participant1Id, Guid participant2Id, DateTimeOffset lastMessageAt)
    {
        Id = id;
        Participant1Id = participant1Id;
        Participant2Id = participant2Id;
        LastMessageAt = lastMessageAt;
    }

    public static Conversation Create(Guid participant1Id, Guid participant2Id, DateTimeOffset createdAt)
    {
        return new Conversation(Guid.NewGuid(), participant1Id, participant2Id, createdAt);
    }

    public void UpdateLastMessageAt(DateTimeOffset timestamp)
    {
        if (timestamp > LastMessageAt)
        {
            LastMessageAt = timestamp;
        }
    }
}
