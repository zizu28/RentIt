using Microsoft.AspNetCore.Components.Forms;
using RentIt.Shared.DTOs.Properties;

namespace RentIt.Client.Web.Services;

public class MockPropertyService : IPropertyService
{
    private readonly List<PropertyDto> _properties = 
    [
        new PropertyDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Luxury Villa with Ocean View",
            Description = "Experience the ultimate getaway in this stunning luxury villa featuring breathtaking ocean views, a private infinity pool, and modern amenities. Perfect for families or couples seeking a romantic retreat.",
            PricePerPeriod = 450.00m,
            City = "Malibu",
            Region = "California",
            Images =
            [ 
                "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?auto=format&fit=crop&q=80&w=800",
                "https://images.unsplash.com/photo-1512918728675-ed5a9ecdebfd?auto=format&fit=crop&q=80&w=800" 
            ],
            MaxGuests = 8,
            Rating = 4.9,
            Amenities = [ "Wifi", "Pool", "Kitchen", "Air conditioning", "Free parking" ]
        },
        new PropertyDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Cozy Downtown Loft",
            Description = "Stay in the heart of the city in this stylish, industrial-chic loft. Walking distance to the best restaurants, cafes, and nightlife.",
            PricePerPeriod = 120.00m,
            City = "New York City",
            Region = "NY",
            Images = new List<string> { 
                "https://images.unsplash.com/photo-1554995207-c18c203602cb?auto=format&fit=crop&q=80&w=800" 
            },
            MaxGuests = 2,
            Rating = 4.7,
            Amenities = new List<string> { "Wifi", "Kitchen", "Heating", "TV" }
        },
        new PropertyDto
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Serene Forest Cabin",
            Description = "Disconnect from the world in this cozy A-frame cabin surrounded by towering pines. Includes a wood-burning stove and a hot tub under the stars.",
            PricePerPeriod = 200.00m,
            City = "Aspen",
            Region = "Colorado",
            Images = new List<string> { 
                "https://images.unsplash.com/photo-1542718610-a1d656d1884c?auto=format&fit=crop&q=80&w=800" 
            },
            MaxGuests = 4,
            Rating = 4.8,
            Amenities = new List<string> { "Wifi", "Hot tub", "Indoor fireplace", "Kitchen" }
        }
    ];

    public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
    {
        await Task.Delay(500); // Simulate network latency
        return _properties;
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(Guid id)
    {
        await Task.Delay(300);
        return _properties.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<PropertyDto>> GetHostPropertiesAsync()
    {
        await Task.Delay(400);
        return _properties; // For mock, just return all as host properties
    }

    public async Task<Guid> CreatePropertyAsync(CreatePropertyRequest request, IEnumerable<IBrowserFile> images)
    {
        await Task.Delay(600);
        var id = Guid.NewGuid();
        _properties.Add(new PropertyDto
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            PricePerPeriod = request.PricePerPeriod,
            City = request.City,
            Region = request.Region,
            Images = request.Images.ToList(),
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            MaxGuests = request.Bedrooms * 2, // arbitrary
            Amenities = request.Amenities.ToList()
        });
        return id;
    }

    public Task UpdatePropertyAsync(Guid id, UpdatePropertyRequest request)
    {
        throw new NotImplementedException();
    }
}
