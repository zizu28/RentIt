using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Shared.Kernel.ValueObjects;

/// <summary>
/// Date range value object
/// </summary>
public sealed record DateRange : ValueObject
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }

    private DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before or equal to end date");

        return new DateRange(startDate, endDate);
    }

    public int DurationInDays() => EndDate.DayNumber - StartDate.DayNumber + 1;

    public bool Contains(DateOnly date) => date >= StartDate && date <= EndDate;

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}
