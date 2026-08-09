using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Properties.Application.Commands;
using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Kernel.Enums;
using RentIt.Shared.Kernel.ValueObjects;

namespace RentIt.Modules.Properties.Application.Handlers;

internal sealed class UpdatePropertyCommandHandler(
    IPropertyRepository propertyRepository,
    [FromKeyedServices("Properties")] IUnitOfWork unitOfWork,
    Serilog.ILogger logger) : IRequestHandler<UpdatePropertyCommand, Result<Guid>>
{
    private readonly IPropertyRepository _propertyRepository = propertyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Result<Guid>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (property == null)
        {
            _logger.Warning("Property with ID {PropertyId} not found.", request.PropertyId);
            return Result.Failure<Guid>("Property not found.");
        }

        if (property.HostId != request.HostId)
        {
            _logger.Warning("User {UserId} attempted to update property {PropertyId} they do not own.", request.HostId, request.PropertyId);
            return Result.Failure<Guid>("You do not have permission to edit this property.");
        }

        var address = Address.Create(request.Street, request.City, request.Region, request.PostalCode, request.Country);
        var price = Money.Create(request.PricePerPeriod, Currency.GHS); // Currently hardcoded to USD like create
        var deposit = Money.Create(request.SecurityDeposit, Currency.GHS);

        property.UpdateDetails(
            request.Name, 
            request.Description, 
            (PropertyType)request.Type,
            request.Bedrooms, 
            request.Bathrooms
        );

        property.UpdateAddress(address);
        
        property.UpdatePricing(price, deposit, (RentalPeriod)request.RentalPeriod);
        
        property.ClearAmenities();
        if (request.Amenities != null)
        {
            property.AddAmenities(request.Amenities);
        }

        property.ChangeStatus((PropertyStatus)request.Status);

        await _propertyRepository.UpdateAsync(property, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Information("Property {PropertyId} updated successfully.", property.Id);

        return Result.Success<Guid>(property.Id);
    }
}
