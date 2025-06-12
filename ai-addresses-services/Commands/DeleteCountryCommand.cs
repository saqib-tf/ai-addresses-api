using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Commands
{
    public record DeleteCountryCommand(int Id) : IRequest<bool>;

    public class DeleteCountryHandler : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly IRepository<Country> _countryRepository;

        public DeleteCountryHandler(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(request.Id);
            if (country == null)
                return false;

            _countryRepository.Remove(country);
            await _countryRepository.SaveChangesAsync();
            return true;
        }
    }
}