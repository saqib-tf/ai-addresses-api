using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record GetAllAddressesQuery() : IRequest<IEnumerable<Address>>;

    public class GetAllAddressesHandler : IRequestHandler<GetAllAddressesQuery, IEnumerable<Address>>
    {
        private readonly IRepository<Address> _addressRepository;

        public GetAllAddressesHandler(IRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<Address>> Handle(GetAllAddressesQuery request, CancellationToken cancellationToken)
        {
            return await _addressRepository.GetAllAsync();
        }
    }
}