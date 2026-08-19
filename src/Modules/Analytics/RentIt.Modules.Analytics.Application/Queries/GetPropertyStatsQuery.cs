using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Analytics.Application.Queries;

public record GetPropertyStatsQuery(Guid PropertyId) : IRequest<Result<PropertyStatsDto>>;

public record PropertyStatsDto(Guid PropertyId, int TotalBookings, int TotalReviews, double AverageRating);
