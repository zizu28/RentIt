using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Shared.Abstractions.Domain;
using RentIt.Shared.Kernel.ValueObjects;
using RentIt.Modules.Properties.Domain.Events;
using System.Text.Json.Serialization;

namespace RentIt.Modules.Properties.Domain.Entities;

public class Property : AggregateRoot<Guid>
{
    public Guid HostId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Address Address { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PropertyType Type { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PropertyStatus Status { get; private set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RentalPeriod RentalPeriod { get; private set; }
    public Money PricePerPeriod { get; private set; }
    public int Bedrooms { get; private set; }
    public int Bathrooms { get; private set; }
    
    private readonly List<string> _amenities = [];
    public IReadOnlyCollection<string> Amenities => _amenities.AsReadOnly();
    
    private readonly List<string> _images = [];
    public IReadOnlyCollection<string> Images => _images.AsReadOnly();

#pragma warning disable CS8618
    private Property() { } // EF Core required
#pragma warning restore CS8618

    private Property(
        Guid id, 
        Guid hostId, 
        string name, 
        string description, 
        Address address, 
        PropertyType type, 
        RentalPeriod rentalPeriod,
        Money pricePerPeriod,
        int bedrooms,
        int bathrooms) : base(id)
    {
        HostId = hostId;
        Name = name;
        Description = description;
        Address = address;
        Type = type;
        RentalPeriod = rentalPeriod;
        PricePerPeriod = pricePerPeriod;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        Status = PropertyStatus.Draft;
    }

    public static Property Create(
        Guid hostId, 
        string name, 
        string description, 
        Address address, 
        PropertyType type, 
        RentalPeriod rentalPeriod,
        Money pricePerPeriod,
        int bedrooms,
        int bathrooms)
    {
        var property = new Property(
            Guid.NewGuid(), 
            hostId, 
            name, 
            description, 
            address, 
            type, 
            rentalPeriod,
            pricePerPeriod,
            bedrooms,
            bathrooms);
            
        property.AddDomainEvent(new PropertyCreatedDomainEvent(
            property.Id,
            property.HostId,
            property.Name,
            property.Type,
            property.RentalPeriod,
            property.PricePerPeriod));
            
        return property;
    }

    public void UpdateDetails(
        string name, 
        string description, 
        PropertyType type, 
        int bedrooms, 
        int bathrooms)
    {
        Name = name;
        Description = description;
        Type = type;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
    }

    public void UpdatePricing(Money pricePerPeriod, RentalPeriod rentalPeriod)
    {
        var oldPrice = PricePerPeriod;
        var oldPeriod = RentalPeriod;
        
        PricePerPeriod = pricePerPeriod;
        RentalPeriod = rentalPeriod;
        
        AddDomainEvent(new PropertyPricingUpdatedDomainEvent(
            Id,
            oldPeriod,
            rentalPeriod,
            oldPrice,
            pricePerPeriod));
    }

    public void ChangeStatus(PropertyStatus newStatus)
    {
        if (Status != newStatus)
        {
            var oldStatus = Status;
            Status = newStatus;
            
            AddDomainEvent(new PropertyStatusChangedDomainEvent(Id, oldStatus, newStatus));
        }
    }

    public void AddAmenity(string amenity)
    {
        if (!string.IsNullOrWhiteSpace(amenity) && !_amenities.Contains(amenity))
        {
            _amenities.Add(amenity.Trim());
        }
    }

    public void RemoveAmenity(string amenity)
    {
        _amenities.Remove(amenity);
    }
    
    public void AddAmenities(IEnumerable<string> amenities)
    {
        foreach (var amenity in amenities)
        {
            AddAmenity(amenity);
        }
    }

    public void AddImage(string imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(imageUrl) && !_images.Contains(imageUrl))
        {
            _images.Add(imageUrl.Trim());
        }
    }

    public void RemoveImage(string imageUrl)
    {
        _images.Remove(imageUrl);
    }
    
    public void MarkAsAvailable()
    {
        if (Status == PropertyStatus.Draft || Status == PropertyStatus.Unlisted || Status == PropertyStatus.Maintenance)
        {
            var oldStatus = Status;
            Status = PropertyStatus.Available;
            
            AddDomainEvent(new PropertyStatusChangedDomainEvent(Id, oldStatus, Status));
        }
    }
}
