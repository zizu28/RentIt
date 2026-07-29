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
    public int MaxGuests { get; init; } = 4;
    public double Rating { get; init; } = 4.5;
    public List<string> Amenities { get; init; } = [];
    public List<string> Images { get; init; } = [];

    // Computed properties for UI compatibility
    [System.Text.Json.Serialization.JsonIgnore]
    public string Title => Name;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public string Location => string.IsNullOrWhiteSpace(City) && string.IsNullOrWhiteSpace(Region) 
        ? "Unknown Location" 
        : $"{City}, {Region}".Trim(',', ' ');
        
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal PricePerNight => PricePerPeriod;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public List<string> ImageUrls => Images ?? [];
}
