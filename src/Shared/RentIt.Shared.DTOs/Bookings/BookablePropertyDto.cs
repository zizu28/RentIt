namespace RentIt.Shared.DTOs.Bookings;

public record BookablePropertyDto(
    Guid Id,
    string Title,
    string ImageUrl,
    decimal PricePerNight,
    string Currency
);
