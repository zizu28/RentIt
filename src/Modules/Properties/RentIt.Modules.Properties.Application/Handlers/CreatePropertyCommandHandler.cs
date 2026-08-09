using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Modules.Properties.Application.Commands;
using RentIt.Modules.Properties.Application.Services;
using RentIt.Modules.Properties.Domain.Entities;
using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Storage;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;
using RentIt.Shared.Kernel.ValueObjects;

namespace RentIt.Modules.Properties.Application.Handlers;

internal sealed class CreatePropertyCommandHandler(
    IPropertyRepository propertyRepository,
    IEventBus eventBus,
    [FromKeyedServices("Properties")] IUnitOfWork unitOfWork,
    Serilog.ILogger logger,
    IBackgroundJob backgroundJob,
    IStorageService storageService) : IRequestHandler<CreatePropertyCommand, Result<Guid>>
{
    private readonly IPropertyRepository _propertyRepository = propertyRepository;
    private readonly IEventBus _eventBus = eventBus;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;
    private readonly IStorageService _storageService = storageService;

    public async Task<Result<Guid>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
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
        var securityDeposit = Money.Cedis(request.SecurityDeposit);

        var property = Property.Create(
            request.HostId,
            request.Name,
            request.Description,
            address,
            (PropertyType)request.Type,
            (RentalPeriod)request.RentalPeriod,
            price,
            securityDeposit,
            request.Bedrooms,
            request.Bathrooms,
            (PropertyStatus)request.Status
        );

        if (request.Amenities != null && request.Amenities.Any())
        {
            property.AddAmenities(request.Amenities);
        }

        if (request.Images != null && request.Images.Any())
        {
            foreach (var img in request.Images)
            {
                var url = await _storageService.UploadImageAsync(img.Content, img.FileName, cancellationToken);
                property.AddImage(url);
            }
        }

            await _propertyRepository.AddAsync(property, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            if(property.Status == PropertyStatus.Available)
            {
                var integrationEvent = new PropertyPublishedIntegrationEvent(
                property.Id,
                property.HostId,
                property.Name,
                property.Address.City,
                property.Address.Region,
                property.PricePerPeriod.Amount,
                property.PricePerPeriod.Currency.ToString(),
                property.Images.FirstOrDefault() ?? string.Empty,
                (int)property.RentalPeriod);

                await _eventBus.PublishAsync(integrationEvent, cancellationToken);
            }

            _logger.Information("Successfully created property {PropertyId} for Host {HostId}", property.Id, property.HostId);
            
            // Background job enqueue using the 'alpha' job name queue to send email to the host (renter)
            _backgroundJob.Enqueue<IPropertyEmailService>("alpha", emailService => emailService.SendPropertyCreationEmailAsync(property.HostId, property.Id, CancellationToken.None));

            return Result.Success<Guid>(property.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating property for Host {HostId}", request.HostId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure<Guid>(Error.Failure("Property.CreateFailed", "An error occurred while creating the property."));
        }
    }
}
