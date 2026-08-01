using MediatR;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Properties;

namespace RentIt.Modules.Properties.Application.Handlers;

internal sealed class GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository) : IRequestHandler<Queries.GetPropertyByIdQuery, Result<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository = propertyRepository;

    public async Task<Result<PropertyDto>> Handle(Queries.GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (property == null)
        {
            return Result.Failure<PropertyDto>("Property not found");           
        }

        var dto = new PropertyDto
        {
            Id = property.Id,
            HostId = property.HostId,
            Name = property.Name,
            Description = property.Description,
            AddressLine1 = property.Address.Street,
            AddressLine2 = string.Empty,
            City = property.Address.City,
            Region = property.Address.Region,
            Country = property.Address.Country,
            PostalCode = property.Address.PostalCode ?? string.Empty,
            Latitude = null,
            Longitude = null,
            Type = (int)property.Type,
            Status = (int)property.Status,
            RentalPeriod = (int)property.RentalPeriod,
            PricePerPeriod = property.PricePerPeriod.Amount,
            SecurityDeposit = property.SecurityDeposit.Amount,
            Currency = property.PricePerPeriod.Currency.ToString(),
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            Amenities = property.Amenities.ToList(),
            Images = property.Images.ToList()
        };

        return Result.Success(dto);
    }
}
