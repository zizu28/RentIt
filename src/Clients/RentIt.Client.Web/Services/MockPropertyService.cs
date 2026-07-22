using RentIt.Shared.DTOs.Properties;

namespace RentIt.Client.Web.Services;

public class MockPropertyService : IPropertyService
{
    private readonly List<PropertyDto> _properties = new()
    {
        new PropertyDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Title = "Luxury Villa with Ocean View",
            Description = "Experience the ultimate getaway in this stunning luxury villa featuring breathtaking ocean views, a private infinity pool, and modern amenities. Perfect for families or couples seeking a romantic retreat.",
            PricePerNight = 450.00m,
            Location = "Malibu, California",
            ImageUrls = new List<string> { 
                "https://images.unsplash.com/photo-1613490900233-141c5560d75d?auto=format&fit=crop&q=80&w=800",
                "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&q=80&w=800" 
            },
            MaxGuests = 8,
            Rating = 4.9,
            Amenities = new List<string> { "Wifi", "Pool", "Kitchen", "Air conditioning", "Free parking" }
        },
        new PropertyDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Title = "Cozy Downtown Loft",
            Description = "Stay in the heart of the city in this stylish, industrial-chic loft. Walking distance to the best restaurants, cafes, and nightlife.",
            PricePerNight = 120.00m,
            Location = "New York City, NY",
            ImageUrls = new List<string> { 
                "https://images.unsplash.com/photo-1502672260266-1c1f51270239?auto=format&fit=crop&q=80&w=800" 
            },
            MaxGuests = 2,
            Rating = 4.7,
            Amenities = new List<string> { "Wifi", "Kitchen", "Heating", "TV" }
        },
        new PropertyDto
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Title = "Serene Forest Cabin",
            Description = "Disconnect from the world in this cozy A-frame cabin surrounded by towering pines. Includes a wood-burning stove and a hot tub under the stars.",
            PricePerNight = 200.00m,
            Location = "Aspen, Colorado",
            ImageUrls = new List<string> { 
                "https://images.unsplash.com/photo-1449844908441-8829872d2607?auto=format&fit=crop&q=80&w=800" 
            },
            MaxGuests = 4,
            Rating = 4.8,
            Amenities = new List<string> { "Wifi", "Hot tub", "Indoor fireplace", "Kitchen" }
        }
    };

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
}
