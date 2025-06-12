using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    // Query
    public record GetAllCountriesQuery() : IRequest<IEnumerable<Country>>;

    // Handler
    public class GetAllCountriesHandler : IRequestHandler<GetAllCountriesQuery, IEnumerable<Country>>
    {
        private readonly IRepository<Country> _countryRepository;

        public GetAllCountriesHandler(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<Country>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            return await _countryRepository.GetAllAsync();
        }
    }
}