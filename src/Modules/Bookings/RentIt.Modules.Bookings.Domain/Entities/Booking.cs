using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Shared.Abstractions.Domain;
using RentIt.Shared.Kernel.Enums;
using RentIt.Shared.Kernel.ValueObjects;

namespace RentIt.Modules.Bookings.Domain.Entities;

public sealed class Booking : AggregateRoot<Guid>
{
    public Guid PropertyId { get; init; }
    public Guid GuestId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public Money TotalPrice { get; init; }
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
        decimal pricePerPeriod,
        string currency,
        int rentalPeriod)
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
        decimal totalPriceAmount;

        if (rentalPeriod == 2) // Monthly
        {
            var months = Math.Max(1, totalDays / 30.0m);
            totalPriceAmount = pricePerPeriod * months;
        }
        else if (rentalPeriod == 3) // Yearly
        {
            var years = Math.Max(1, totalDays / 365.0m);
            totalPriceAmount = pricePerPeriod * years;
        }
        else // Nightly
        {
            totalPriceAmount = pricePerPeriod * totalDays;
        }

        if (!Enum.TryParse<Currency>(currency, true, out var parsedCurrency))
        {
            parsedCurrency = Currency.GHS; // Default or throw
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

    public void MarkAsRefunded()
    {
        if (Status == BookingStatus.Completed)
        {
            throw new BookingDomainException($"Booking is {Status} and cannot be refunded.");
        }

        Status = BookingStatus.Refunded;
    }

    public void MarkAsPartiallyPaid()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new BookingDomainException($"Booking is {Status} and cannot be marked as partially paid.");
        }

        Status = BookingStatus.PartiallyPaid;
    }

    public void MarkAsFailed()
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.PartiallyPaid)
        {
            throw new BookingDomainException($"Booking is {Status} and cannot be marked as failed.");
        }

        Status = BookingStatus.Failed;
    }
}
