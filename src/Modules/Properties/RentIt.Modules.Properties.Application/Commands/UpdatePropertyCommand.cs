using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Properties.Application.Commands;

public sealed record UpdatePropertyCommand(
    Guid PropertyId,
    Guid HostId,
    string Name,
    string Description,
    string Street,
    string City,
    string Region,
    string Country,
    string PostalCode,
    int Type,
    int RentalPeriod,
    decimal PricePerPeriod,
    int Bedrooms,
    int Bathrooms,
    IEnumerable<string> Amenities
) : IRequest<Result<Guid>>;
