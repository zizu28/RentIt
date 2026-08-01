using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Bookings.Domain.Entities;

public class BookableProperty : Entity<Guid>
{
    public string Title { get; private set; }
    public string ImageUrl { get; private set; }
    public decimal PricePerNight { get; private set; }
    public string Currency { get; private set; }

#pragma warning disable CS8618
    private BookableProperty() { } // EF Core required
#pragma warning restore CS8618

    public BookableProperty(Guid id, string title, string imageUrl, decimal pricePerNight, string currency)
    {
        Id = id;
        Title = title;
        ImageUrl = imageUrl;
        PricePerNight = pricePerNight;
        Currency = currency;
    }

    public void Update(string title, decimal pricePerNight, string currency)
    {
        Title = title;
        PricePerNight = pricePerNight;
        Currency = currency;
    }
}
