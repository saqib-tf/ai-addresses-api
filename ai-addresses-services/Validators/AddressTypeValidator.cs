using ai_addresses_data.Entity;
using FluentValidation;

namespace ai_addresses_services.Validators
{
    public class AddressTypeValidator : AbstractValidator<AddressType>
    {
        public AddressTypeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(50).WithMessage("Name must be at most 50 characters.");
        }
    }
}