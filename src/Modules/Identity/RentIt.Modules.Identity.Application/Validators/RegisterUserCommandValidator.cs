using FluentValidation;
using RentIt.Modules.Identity.Application.Commands;

namespace RentIt.Modules.Identity.Application.Validators;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?233[0-9]{9}$|^0[0-9]{9}$")
            .WithMessage("Phone number must be a valid Ghana number");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => new[] { "Renter", "Host", "Agent" }.Contains(role))
            .WithMessage("Role must be Renter, Host, or Agent");
    }
}
