using ai_addresses_data.Entity;
using FluentValidation;

namespace ai_addresses_services.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required.")
                .MinimumLength(2).WithMessage("Street must be at least 2 characters.")
                .MaximumLength(200).WithMessage("Street must be at most 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MinimumLength(2).WithMessage("City must be at least 2 characters.")
                .MaximumLength(100).WithMessage("City must be at most 100 characters.");

            RuleFor(x => x.PostalCode)
                .MaximumLength(20).WithMessage("Postal code must be at most 20 characters.");

            RuleFor(x => x.PersonId)
                .GreaterThan(0).WithMessage("PersonId is required and must be greater than 0.");

            // Optionally, you can add rules for StateId and AddressTypeId if needed
        }
    }
}