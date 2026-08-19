using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Reviews.Application.Queries;

public record GetPropertyReviewsQuery(Guid PropertyId) : IRequest<Result<IEnumerable<ReviewDto>>>;

public record ReviewDto(Guid Id, Guid GuestId, int Rating, string Comment, DateTime CreatedAt);
