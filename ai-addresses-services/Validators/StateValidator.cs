using ai_addresses_data.Entity;
using FluentValidation;

namespace ai_addresses_services.Validators
{
    public class StateValidator : AbstractValidator<State>
    {
        public StateValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MinimumLength(2).WithMessage("Code must be at least 2 characters.")
                .MaximumLength(10).WithMessage("Code must be at most 10 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("CountryId is required and must be greater than 0.");
        }
    }
}