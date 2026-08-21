using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Analytics.Domain.Entities;

public class HostMetrics : AggregateRoot<Guid>
{
    public Guid HostId { get; private set; }
    public int TotalProperties { get; private set; }
    public int TotalBookings { get; private set; }
    public decimal TotalRevenue { get; private set; }
    public int TotalReviews { get; private set; }
    public double AverageRating { get; private set; }

#pragma warning disable CS8618
    private HostMetrics() { }
#pragma warning restore CS8618

    private HostMetrics(Guid id, Guid hostId) : base(id)
    {
        HostId = hostId;
        TotalProperties = 0;
        TotalBookings = 0;
        TotalRevenue = 0;
        TotalReviews = 0;
        AverageRating = 0;
    }

    public static HostMetrics Create(Guid hostId)
    {
        return new HostMetrics(Guid.NewGuid(), hostId);
    }

    public void IncrementProperties()
    {
        TotalProperties++;
    }

    public void IncrementBookings()
    {
        TotalBookings++;
    }

    public void AddRevenue(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }
        TotalRevenue += amount;
    }

    public void DeductRevenue(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }
        TotalRevenue -= amount;
    }

    public void AddReview(int rating)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
        }
        var totalScore = (AverageRating * TotalReviews) + rating;
        TotalReviews++;
        AverageRating = totalScore / TotalReviews;
    }
}
