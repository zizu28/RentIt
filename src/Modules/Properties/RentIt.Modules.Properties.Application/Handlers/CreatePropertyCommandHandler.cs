using MediatR;
using RentIt.Modules.Properties.Domain.Entities;
using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.BackgroundJobs;
using Serilog;
using RentIt.Modules.Properties.Application.Services;
using RentIt.Shared.Kernel.Enums;
using RentIt.Shared.Kernel.ValueObjects;

namespace RentIt.Modules.Properties.Application.Handlers;

internal sealed class CreatePropertyCommandHandler : IRequestHandler<Commands.CreatePropertyCommand, Result<Guid>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IBackgroundJob _backgroundJob;

    public CreatePropertyCommandHandler(
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork,
        ILogger logger,
        IBackgroundJob backgroundJob)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _backgroundJob = backgroundJob;
    }

    public async Task<Result<Guid>> Handle(Commands.CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Attempting to create a new property for Host {HostId}", request.HostId);
        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var address = Address.Create(
            request.Street,
            request.City,
            request.Region,
            request.Country,
            request.PostalCode
        );

        // Assuming GHS as default for now, could be passed in command
        var price = Money.Cedis(request.PricePerPeriod); 

        var property = Property.Create(
            request.HostId,
            request.Name,
            request.Description,
            address,
            (PropertyType)request.Type,
            (RentalPeriod)request.RentalPeriod,
            price,
            request.Bedrooms,
            request.Bathrooms
        );

        if (request.Amenities != null && request.Amenities.Any())
        {
            property.AddAmenities(request.Amenities);
        }

        if (request.Images != null && request.Images.Any())
        {
            foreach (var img in request.Images)
            {
                property.AddImage(img);
            }
        }

            await _propertyRepository.AddAsync(property, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            _logger.Information("Successfully created property {PropertyId} for Host {HostId}", property.Id, property.HostId);
            
            // Background job enqueue using the 'alpha' job name queue to send email to the host (renter)
            _backgroundJob.Enqueue<IPropertyEmailService>("alpha", emailService => emailService.SendPropertyCreationEmailAsync(property.HostId, property.Id, CancellationToken.None));

            return Result<Guid>.Success(property.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating property for Host {HostId}", request.HostId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure<Guid>(Error.Failure("Property.CreateFailed", "An error occurred while creating the property."));
        }
    }
}
