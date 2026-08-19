using RentIt.Modules.Reviews.Domain.Events;
using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Reviews.Domain.Entities;

public class Review : AggregateRoot<Guid>
{
    public Guid PropertyId { get; private set; }
    public Guid GuestId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

#pragma warning disable CS8618
    private Review() { } // EF Core
#pragma warning restore CS8618

    private Review(Guid id, Guid propertyId, Guid guestId, int rating, string comment) : base(id)
    {
        PropertyId = propertyId;
        GuestId = guestId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    public static Review Create(Guid propertyId, Guid guestId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
        }

        var review = new Review(Guid.NewGuid(), propertyId, guestId, rating, comment);
        
        review.AddDomainEvent(new ReviewAddedDomainEvent(
            review.Id,
            review.PropertyId,
            review.GuestId,
            review.Rating));
            
        return review;
    }
}
