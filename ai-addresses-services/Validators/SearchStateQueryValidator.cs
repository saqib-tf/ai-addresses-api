using ai_addresses_services.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_addresses_services.Validators
{
    public class SearchStateQueryValidator : AbstractValidator<SearchStateQuery>
    {
        public SearchStateQueryValidator()
        {
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage("CountryId is required.");
        }
    }
}
