using RentIt.Shared.Abstractions.Messaging;
using System.Text.Json.Serialization;

namespace RentIt.Shared.Contracts.Identity.IntegrationEvents;

public class UserDeletionEligibilityContext
{
    public bool IsEligible { get; set; } = true;
    public List<string> Reasons { get; } = new();

    public void Reject(string reason)
    {
        IsEligible = false;
        Reasons.Add(reason);
    }
}

public sealed record UserDeletionEligibilityIntegrationEvent(
    Guid UserId,
    string Role,
    [property: JsonIgnore] UserDeletionEligibilityContext Context
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
