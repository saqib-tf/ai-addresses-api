using ai_addresses_data.Entity;
using FluentValidation;

namespace ai_addresses_services.Validators
{
    public class GenderValidator : AbstractValidator<Gender>
    {
        public GenderValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Length(1).WithMessage("Code must be exactly 1 character.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.");
        }
    }
}