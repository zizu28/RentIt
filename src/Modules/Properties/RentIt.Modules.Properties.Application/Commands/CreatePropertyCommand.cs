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
    decimal SecurityDeposit,
    int Bedrooms,
    int Bathrooms,
    IEnumerable<string> Amenities,
    IEnumerable<CreatePropertyCommand.FileRecord> Images,
    int Status = 1 // Default to Draft
) : IRequest<Result<Guid>>
{
    public record FileRecord(Stream Content, string FileName);
}
