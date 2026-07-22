namespace RentIt.Shared.DTOs.Properties;

public sealed record PropertyDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal PricePerNight { get; init; }
    public string Location { get; init; } = string.Empty;
    public List<string> ImageUrls { get; init; } = new();
    public int MaxGuests { get; init; }
    public double Rating { get; init; }
    public List<string> Amenities { get; init; } = new();
}
