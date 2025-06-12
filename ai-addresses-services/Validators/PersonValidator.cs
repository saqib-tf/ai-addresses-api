using ai_addresses_data.Entity;
using FluentValidation;

namespace ai_addresses_services.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("First name must be at most 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Last name must be at most 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.");

            RuleFor(x => x.ProfilePictureUrl)
                .MaximumLength(500).WithMessage("Profile picture URL must be at most 500 characters.");
        }
    }
}