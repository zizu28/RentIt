using MediatR;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class CreateBookablePropertyCommandHandler(
    IBookablePropertyRepository repository,
    IUnitOfWork unitOfWork,
    Serilog.ILogger logger) : IRequestHandler<CreateBookablePropertyCommand, Result<Guid>>
{
    private readonly IBookablePropertyRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Result<Guid>> Handle(CreateBookablePropertyCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Attempting to create bookable property {PropertyId}", request.PropertyId);
        
        var existing = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (existing != null)
        {
            _logger.Warning("Bookable property {PropertyId} already exists.", request.PropertyId);
            return Result.Failure<Guid>("Bookable property already exists.");
        }

        var property = new BookableProperty(
            request.PropertyId,
            request.Title,
            request.ImageUrl,
            request.PricePerNight,
            request.Currency
        );

        _repository.Add(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Information("Successfully created bookable property {PropertyId}", request.PropertyId);
        return Result.Success<Guid>(property.Id);
    }
}
