using FluentValidation;
using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Reviews.Application.Commands;

public record AddReviewCommand(Guid PropertyId, Guid GuestId, int Rating, string Comment) : IRequest<Result<Guid>>;

public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewCommandValidator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.GuestId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}
