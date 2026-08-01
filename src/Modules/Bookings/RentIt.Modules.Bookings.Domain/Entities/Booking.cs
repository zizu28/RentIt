using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Shared.Abstractions.Domain;
using RentIt.Shared.Kernel.ValueObjects;
using RentIt.Modules.Bookings.Domain.Exceptions;

namespace RentIt.Modules.Bookings.Domain.Entities;

public class Booking : AggregateRoot<Guid>
{
    public Guid PropertyId { get; private set; }
    public Guid GuestId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public Money TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }

#pragma warning disable CS8618
    private Booking() { } // EF Core required
#pragma warning restore CS8618

    private Booking(
        Guid id, 
        Guid propertyId, 
        Guid guestId, 
        DateOnly startDate, 
        DateOnly endDate, 
        Money totalPrice, 
        BookingStatus status)
    {
        Id = id;
        PropertyId = propertyId;
        GuestId = guestId;
        StartDate = startDate;
        EndDate = endDate;
        TotalPrice = totalPrice ?? throw new BookingDomainException("Total price is required.");
        Status = status;
    }

    public static Booking Create(
        Guid propertyId,
        Guid guestId,
        DateOnly startDate,
        DateOnly endDate,
        decimal pricePerNight,
        string currency)
    {
        if (startDate >= endDate)
        {
            throw new BookingDomainException("Start date must be before end date.");
        }

        if (startDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BookingDomainException("Start date cannot be in the past.");
        }

        var totalDays = endDate.DayNumber - startDate.DayNumber;
        var totalPriceAmount = pricePerNight * totalDays;
        if (!Enum.TryParse<RentIt.Shared.Kernel.Enums.Currency>(currency, true, out var parsedCurrency))
        {
            parsedCurrency = RentIt.Shared.Kernel.Enums.Currency.USD; // Default or throw
        }

        var totalPrice = Money.Create(totalPriceAmount, parsedCurrency);

        // Set to Pending until payment is processed
        var booking = new Booking(
            Guid.NewGuid(),
            propertyId,
            guestId,
            startDate,
            endDate,
            totalPrice,
            BookingStatus.Pending);

        return booking;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new BookingDomainException($"Booking is {Status} and cannot be confirmed.");
        }
        
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled || Status == BookingStatus.Completed)
        {
            throw new BookingDomainException($"Booking is {Status} and cannot be cancelled.");
        }

        Status = BookingStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
        {
            throw new BookingDomainException($"Booking is {Status} and cannot be completed.");
        }

        if (EndDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BookingDomainException("Booking cannot be completed before its end date.");
        }

        Status = BookingStatus.Completed;
    }
}
