using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Analytics.Domain.Entities;

public class PropertyMetrics : AggregateRoot<Guid>
{
    public Guid PropertyId { get; private set; }
    public int TotalBookings { get; private set; }
    public int TotalReviews { get; private set; }
    public double AverageRating { get; private set; }

#pragma warning disable CS8618
    private PropertyMetrics() { }
#pragma warning restore CS8618

    private PropertyMetrics(Guid id, Guid propertyId) : base(id)
    {
        PropertyId = propertyId;
        TotalBookings = 0;
        TotalReviews = 0;
        AverageRating = 0;
    }

    public static PropertyMetrics Create(Guid propertyId)
    {
        return new PropertyMetrics(Guid.NewGuid(), propertyId);
    }

    public void IncrementBookings()
    {
        TotalBookings++;
    }

    public void AddReview(int rating)
    {
        var totalScore = (AverageRating * TotalReviews) + rating;
        TotalReviews++;
        AverageRating = totalScore / TotalReviews;
    }
}
