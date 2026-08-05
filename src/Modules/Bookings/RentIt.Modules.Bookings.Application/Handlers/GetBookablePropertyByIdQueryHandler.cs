using MediatR;
using RentIt.Modules.Bookings.Application.Queries;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class GetBookablePropertyByIdQueryHandler : IRequestHandler<GetBookablePropertyByIdQuery, Result<BookablePropertyDto>>
{
    private readonly IBookablePropertyRepository _repository;
    private readonly Serilog.ILogger _logger;

    public GetBookablePropertyByIdQueryHandler(
        IBookablePropertyRepository repository,
        Serilog.ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<BookablePropertyDto>> Handle(GetBookablePropertyByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.Information("Fetching bookable property details for {PropertyId}", request.Id);
        
        var property = await _repository.GetByIdAsync(request.Id);

        if (property == null)
        {
            _logger.Warning("Bookable property {PropertyId} not found.", request.Id);
            return Result.Failure<BookablePropertyDto>("Bookable property not found.");
        }

        var dto = new BookablePropertyDto(
            property.Id,
            property.Title,
            property.ImageUrl,
            property.PricePerNight,
            property.Currency
        );

        _logger.Information("Successfully fetched bookable property {PropertyId}", request.Id);
        return Result.Success<BookablePropertyDto>(dto);
    }
}
