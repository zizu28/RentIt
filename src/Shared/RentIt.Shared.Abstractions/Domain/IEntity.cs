namespace RentIt.Shared.Abstractions.Domain;

/// <summary>
/// Base interface for all domain entities
/// </summary>
/// <typeparam name="TId">The type of the entity identifier</typeparam>
public interface IEntity<TId>
{
    TId Id { get; }
}
