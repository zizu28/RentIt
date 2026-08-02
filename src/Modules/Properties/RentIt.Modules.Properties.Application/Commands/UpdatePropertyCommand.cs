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
    decimal SecurityDeposit,
    int Bedrooms,
    int Bathrooms,
    IEnumerable<string> Amenities,
    int Status
) : IRequest<Result<Guid>>;
