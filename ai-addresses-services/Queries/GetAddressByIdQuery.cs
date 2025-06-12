using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record GetAddressByIdQuery(int Id) : IRequest<Address?>;

    public class GetAddressByIdHandler : IRequestHandler<GetAddressByIdQuery, Address?>
    {
        private readonly IRepository<Address> _addressRepository;

        public GetAddressByIdHandler(IRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<Address?> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            return await _addressRepository.GetByIdAsync(request.Id);
        }
    }
}