using FluentValidation;
using RentIt.Client.Web.Models;

namespace RentIt.Client.Web.Validators;

public class PropertyFormValidator : AbstractValidator<PropertyFormModel>
{
    public PropertyFormValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Property Name is required.")
            .MaximumLength(100).WithMessage("Property Name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street Address is required.");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region/State is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.");

        RuleFor(x => x.PricePerPeriod)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.SecurityDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Security Deposit cannot be negative.");

        RuleFor(x => x.Bedrooms)
            .GreaterThan(0).WithMessage("Property must have at least 1 bedroom.");

        RuleFor(x => x.Bathrooms)
            .GreaterThan(0).WithMessage("Property must have at least 1 bathroom.");
    }
}
