using MediatR;
using RentIt.Modules.Reviews.Domain.Entities;
using RentIt.Modules.Reviews.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Reviews.Application.Commands;

internal sealed class AddReviewCommandHandler(
    [FromKeyedServices("Reviews")] IUnitOfWork unitOfWork, 
    IReviewRepository reviewRepository) : IRequestHandler<AddReviewCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IReviewRepository _reviewRepository = reviewRepository;

    public async Task<Result<Guid>> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {

        var review = Review.Create(request.PropertyId, request.GuestId, request.Rating, request.Comment);

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid>(review.Id);
    }
}
