namespace RentIt.Shared.DTOs.Properties;

public sealed record PropertyDto
{
    public Guid Id { get; init; }
    public Guid HostId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string AddressLine2 { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public int Type { get; init; } // PropertyType enum
    public int Status { get; init; } // PropertyStatus enum
    public int RentalPeriod { get; init; } // RentalPeriod enum
    public decimal PricePerPeriod { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public List<string> Amenities { get; init; } = new();
    public List<string> Images { get; init; } = new();
}
