using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record CreateAddressTypeCommand(AddressType AddressType) : IRequest<AddressType>;

    public class CreateAddressTypeHandler : IRequestHandler<CreateAddressTypeCommand, AddressType>
    {
        private readonly IRepository<AddressType> _addressTypeRepository;
        private readonly IValidator<AddressType> _validator;

        public CreateAddressTypeHandler(
            IRepository<AddressType> addressTypeRepository,
            IValidator<AddressType> validator
        )
        {
            _addressTypeRepository = addressTypeRepository;
            _validator = validator;
        }

        public async Task<AddressType> Handle(
            CreateAddressTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            var addressType = request.AddressType;

            var validationResult = await _validator.ValidateAsync(addressType, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _addressTypeRepository.AddAsync(addressType);
            await _addressTypeRepository.SaveChangesAsync();
            return addressType;
        }
    }
}
