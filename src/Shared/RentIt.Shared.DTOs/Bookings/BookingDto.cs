namespace RentIt.Shared.DTOs.Bookings;

public sealed record BookingDto
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }
    public string PropertyTitle { get; init; } = string.Empty;
    public string PropertyImageUrl { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public decimal TotalPrice { get; init; }
    public string Status { get; init; } = "Confirmed";
}
