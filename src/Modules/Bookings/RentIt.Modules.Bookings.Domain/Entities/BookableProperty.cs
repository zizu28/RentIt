using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Bookings.Domain.Entities;

public class BookableProperty : Entity<Guid>
{
    public string Title { get; private set; }
    public string ImageUrl { get; private set; }
    public decimal PricePerNight { get; private set; }
    public string Currency { get; private set; }
    public int RentalPeriod { get; private set; }
    public Guid HostId { get; private set; }

#pragma warning disable CS8618
    private BookableProperty() { } // EF Core required
#pragma warning restore CS8618

    public BookableProperty(Guid id, string title, string imageUrl, decimal pricePerNight, string currency, int rentalPeriod, Guid hostId)
    {
        Id = id;
        Title = title;
        ImageUrl = imageUrl;
        PricePerNight = pricePerNight;
        Currency = currency;
        RentalPeriod = rentalPeriod;
        HostId = hostId;
    }

    public void Update(string title, string imageUrl, decimal pricePerNight, string currency, int rentalPeriod, Guid hostId)
    {
        Title = title;
        ImageUrl = imageUrl;
        PricePerNight = pricePerNight;
        Currency = currency;
        RentalPeriod = rentalPeriod;
        HostId = hostId;
    }
}
