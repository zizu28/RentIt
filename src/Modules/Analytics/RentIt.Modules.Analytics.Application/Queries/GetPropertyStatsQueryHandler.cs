using MediatR;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Analytics.Application.Queries;

internal sealed class GetPropertyStatsQueryHandler(IPropertyMetricsRepository repository) : IRequestHandler<GetPropertyStatsQuery, Result<PropertyStatsDto>>
{
    private readonly IPropertyMetricsRepository _repository = repository;

    public async Task<Result<PropertyStatsDto>> Handle(GetPropertyStatsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _repository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        if (metrics is null)
        {
            return Result.Success(new PropertyStatsDto(request.PropertyId, 0, 0, 0.0));
        }

        return Result.Success(new PropertyStatsDto(
            metrics.PropertyId,
            metrics.TotalBookings,
            metrics.TotalReviews,
            metrics.AverageRating
        ));
    }
}
