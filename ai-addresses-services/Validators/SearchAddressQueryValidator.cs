using ai_addresses_services.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_addresses_services.Validators
{
    public class SearchAddressQueryValidator : AbstractValidator<SearchAddressQuery>
    {
        public SearchAddressQueryValidator()
        {
            RuleFor(x => x.PersonId)
                .NotNull()
                .WithMessage("PersonId is required.");
            RuleFor(x => x.PersonId)
                .GreaterThan(0)
                .WithMessage("PersonId must be greater than 0.");
        }
    }
}
