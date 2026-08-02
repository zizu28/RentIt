namespace RentIt.Shared.Abstractions.Messaging;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime OccurredOn { get; init; }
    public DateTime? ProcessedOn { get; private set; }
    public string? Error { get; private set; }
    
    public void MarkAsProcessed() => ProcessedOn = DateTime.UtcNow;
    public void MarkAsFailed(string error) => Error = error;
}
