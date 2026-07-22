namespace RentIt.Shared.Abstractions.Domain;

/// <summary>
/// Base interface for aggregate roots
/// </summary>
/// <typeparam name="TId">The type of the aggregate root identifier</typeparam>
public interface IAggregateRoot<TId> : IEntity<TId>
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    byte[] RowVersion { get; set; }
    void ClearDomainEvents();
}
