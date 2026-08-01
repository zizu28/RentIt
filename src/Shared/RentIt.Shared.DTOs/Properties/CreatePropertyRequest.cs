namespace RentIt.Shared.DTOs.Properties;

public sealed record CreatePropertyRequest(
    string Name,
    string Description,
    string Street,
    string City,
    string Region,
    string Country,
    string PostalCode,
    int Type,
    int RentalPeriod,
    decimal PricePerPeriod,
    decimal SecurityDeposit,
    int Bedrooms,
    int Bathrooms,
    IEnumerable<string> Amenities,
    IEnumerable<string> Images
);
