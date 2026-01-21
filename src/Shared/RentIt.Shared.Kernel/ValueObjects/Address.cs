namespace RentIt.Shared.Kernel.ValueObjects;

/// <summary>
/// Address value object
/// </summary>
public sealed record Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string Region { get; init; }
    public string Country { get; init; }
    public string? PostalCode { get; init; }

    private Address(string street, string city, string region, string country, string? postalCode = null)
    {
        Street = street;
        City = city;
        Region = region;
        Country = country;
        PostalCode = postalCode;
    }

    public static Address Create(string street, string city, string region, string country = "Ghana", string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));

        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be empty", nameof(region));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        return new Address(
            street.Trim(),
            city.Trim(),
            region.Trim(),
            country.Trim(),
            postalCode?.Trim()
        );
    }

    public override string ToString()
    {
        var parts = new List<string> { Street, City, Region, Country };
        
        if (!string.IsNullOrEmpty(PostalCode))
            parts.Add(PostalCode);

        return string.Join(", ", parts);
    }
}
