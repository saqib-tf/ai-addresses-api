using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Commands
{
    public record DeleteAddressTypeCommand(int Id) : IRequest<bool>;

    public class DeleteAddressTypeHandler : IRequestHandler<DeleteAddressTypeCommand, bool>
    {
        private readonly IRepository<AddressType> _addressTypeRepository;

        public DeleteAddressTypeHandler(IRepository<AddressType> addressTypeRepository)
        {
            _addressTypeRepository = addressTypeRepository;
        }

        public async Task<bool> Handle(DeleteAddressTypeCommand request, CancellationToken cancellationToken)
        {
            var addressType = await _addressTypeRepository.GetByIdAsync(request.Id);
            if (addressType == null)
                return false;

            _addressTypeRepository.Remove(addressType);
            await _addressTypeRepository.SaveChangesAsync();
            return true;
        }
    }
}