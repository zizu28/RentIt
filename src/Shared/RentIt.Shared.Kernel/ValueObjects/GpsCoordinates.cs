using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Shared.Kernel.ValueObjects;

/// <summary>
/// GPS Coordinates value object
/// </summary>
public sealed record GpsCoordinates : ValueObject
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    private GpsCoordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GpsCoordinates Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        return new GpsCoordinates(latitude, longitude);
    }

    /// <summary>
    /// Calculates the distance in kilometers between two GPS coordinates using the Haversine formula
    /// </summary>
    public double DistanceInKilometers(GpsCoordinates other)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var lat1 = DegreesToRadians(Latitude);
        var lat2 = DegreesToRadians(other.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";
}
