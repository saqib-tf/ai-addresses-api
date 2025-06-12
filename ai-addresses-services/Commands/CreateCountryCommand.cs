using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record CreateCountryCommand(Country Country) : IRequest<Country>;

    public class CreateCountryHandler : IRequestHandler<CreateCountryCommand, Country>
    {
        private readonly IRepository<Country> _countryRepository;
        private readonly IValidator<Country> _validator;

        public CreateCountryHandler(
            IRepository<Country> countryRepository,
            IValidator<Country> validator
        )
        {
            _countryRepository = countryRepository;
            _validator = validator;
        }

        public async Task<Country> Handle(
            CreateCountryCommand request,
            CancellationToken cancellationToken
        )
        {
            var country = request.Country;

            var validationResult = await _validator.ValidateAsync(country, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _countryRepository.AddAsync(country);
            await _countryRepository.SaveChangesAsync();
            return country;
        }
    }
}
