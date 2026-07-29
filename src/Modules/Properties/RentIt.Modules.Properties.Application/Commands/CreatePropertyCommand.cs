using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Properties.Application.Commands;

public sealed record CreatePropertyCommand(
    Guid HostId,
    string Name,
    string Description,
    string Street,
    string City,
    string Region,
    string Country,
    string PostalCode,
    int Type, // PropertyType enum
    int RentalPeriod, // RentalPeriod enum
    decimal PricePerPeriod,
    int Bedrooms,
    int Bathrooms,
    IEnumerable<string> Amenities,
    IEnumerable<string> Images
) : IRequest<Result<Guid>>;
