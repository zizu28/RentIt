using MediatR;
using RentIt.Modules.Reviews.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Reviews.Application.Queries;

internal sealed class GetPropertyReviewsQueryHandler(IReviewRepository reviewRepository) : IRequestHandler<GetPropertyReviewsQuery, Result<IEnumerable<ReviewDto>>>
{
    private readonly IReviewRepository _reviewRepository = reviewRepository;

    public async Task<Result<IEnumerable<ReviewDto>>> Handle(GetPropertyReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        var dtos = reviews.Select(r => new ReviewDto(
            r.Id,
            r.GuestId,
            r.Rating,
            r.Comment,
            r.CreatedAt
        ));

        return Result.Success<IEnumerable<ReviewDto>>(dtos);
    }
}
