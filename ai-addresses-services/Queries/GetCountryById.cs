using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    // Query
    public record GetCountryByIdQuery(int Id) : IRequest<Country?>;

    // Handler
    public class GetCountryByIdHandler : IRequestHandler<GetCountryByIdQuery, Country?>
    {
        private readonly IRepository<Country> _countryRepository;

        public GetCountryByIdHandler(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<Country?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _countryRepository.GetByIdAsync(request.Id);
        }
    }
}