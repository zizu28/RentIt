using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Messaging.Domain.Entities;

public class MessagingUser : Entity<Guid>
{
    public bool IsSuspended { get; private set; }

#pragma warning disable CS8618
    private MessagingUser() { }
#pragma warning restore CS8618

    private MessagingUser(Guid id, bool isSuspended)
    {
        Id = id;
        IsSuspended = isSuspended;
    }

    public static MessagingUser Create(Guid id)
    {
        return new MessagingUser(id, false);
    }

    public void Suspend()
    {
        IsSuspended = true;
    }

    public void Reactivate()
    {
        IsSuspended = false;
    }
}
