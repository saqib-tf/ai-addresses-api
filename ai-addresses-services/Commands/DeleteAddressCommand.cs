using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Commands
{
    public record DeleteAddressCommand(int Id) : IRequest<bool>;

    public class DeleteAddressHandler : IRequestHandler<DeleteAddressCommand, bool>
    {
        private readonly IRepository<Address> _addressRepository;

        public DeleteAddressHandler(IRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _addressRepository.GetByIdAsync(request.Id);
            if (address == null)
                return false;

            _addressRepository.Remove(address);
            await _addressRepository.SaveChangesAsync();
            return true;
        }
    }
}