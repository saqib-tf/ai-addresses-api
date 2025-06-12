using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record GetAddressTypeByIdQuery(int Id) : IRequest<AddressType?>;

    public class GetAddressTypeByIdHandler : IRequestHandler<GetAddressTypeByIdQuery, AddressType?>
    {
        private readonly IRepository<AddressType> _addressTypeRepository;

        public GetAddressTypeByIdHandler(IRepository<AddressType> addressTypeRepository)
        {
            _addressTypeRepository = addressTypeRepository;
        }

        public async Task<AddressType?> Handle(GetAddressTypeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _addressTypeRepository.GetByIdAsync(request.Id);
        }
    }
}