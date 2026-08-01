namespace RentIt.Modules.Bookings.Domain.Exceptions;

public class BookingDomainException : Exception
{
    public BookingDomainException(string message) : base(message)
    {
    }
}
