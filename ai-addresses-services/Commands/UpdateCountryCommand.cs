using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record UpdateCountryCommand(Country Country) : IRequest<Country?>;

    public class UpdateCountryHandler : IRequestHandler<UpdateCountryCommand, Country?>
    {
        private readonly IRepository<Country> _countryRepository;
        private readonly IValidator<Country> _validator;
        private readonly IMapper _mapper;

        public UpdateCountryHandler(
            IRepository<Country> countryRepository,
            IValidator<Country> validator,
            IMapper mapper
        )
        {
            _countryRepository = countryRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<Country?> Handle(
            UpdateCountryCommand request,
            CancellationToken cancellationToken
        )
        {
            var updatedCountry = request.Country;
            var country = await _countryRepository.GetByIdAsync(updatedCountry.Id);
            if (country == null)
                return null;

            _mapper.Map(updatedCountry, country);

            var validationResult = await _validator.ValidateAsync(country, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _countryRepository.Update(country);
            await _countryRepository.SaveChangesAsync();
            return country;
        }
    }
}
