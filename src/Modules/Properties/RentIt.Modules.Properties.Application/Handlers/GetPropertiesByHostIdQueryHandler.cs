using MediatR;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Properties;

namespace RentIt.Modules.Properties.Application.Handlers;

internal sealed class GetPropertiesByHostIdQueryHandler : IRequestHandler<Queries.GetPropertiesByHostIdQuery, Result<IEnumerable<PropertyDto>>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesByHostIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<IEnumerable<PropertyDto>>> Handle(Queries.GetPropertiesByHostIdQuery request, CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.GetByHostIdAsync(request.HostId, cancellationToken);

        var dtos = properties.Select(p => new PropertyDto
        {
            Id = p.Id,
            HostId = p.HostId,
            Name = p.Name,
            Description = p.Description,
            AddressLine1 = p.Address.Street,
            AddressLine2 = string.Empty,
            City = p.Address.City,
            Region = p.Address.Region,
            Country = p.Address.Country,
            PostalCode = p.Address.PostalCode ?? string.Empty,
            Latitude = null,
            Longitude = null,
            Type = (int)p.Type,
            Status = (int)p.Status,
            RentalPeriod = (int)p.RentalPeriod,
            PricePerPeriod = p.PricePerPeriod.Amount,
            SecurityDeposit = p.SecurityDeposit.Amount,
            Currency = p.PricePerPeriod.Currency.ToString(),
            Bedrooms = p.Bedrooms,
            Bathrooms = p.Bathrooms,
            Amenities = p.Amenities.ToList(),
            Images = p.Images.ToList()
        });

        return Result<IEnumerable<PropertyDto>>.Success(dtos);
    }
}
