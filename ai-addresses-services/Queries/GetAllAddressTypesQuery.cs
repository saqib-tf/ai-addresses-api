using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record GetAllAddressTypesQuery() : IRequest<IEnumerable<AddressType>>;

    public class GetAllAddressTypesHandler : IRequestHandler<GetAllAddressTypesQuery, IEnumerable<AddressType>>
    {
        private readonly IRepository<AddressType> _addressTypeRepository;

        public GetAllAddressTypesHandler(IRepository<AddressType> addressTypeRepository)
        {
            _addressTypeRepository = addressTypeRepository;
        }

        public async Task<IEnumerable<AddressType>> Handle(GetAllAddressTypesQuery request, CancellationToken cancellationToken)
        {
            return await _addressTypeRepository.GetAllAsync();
        }
    }
}