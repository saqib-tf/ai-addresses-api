using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record UpdateAddressTypeCommand(AddressType AddressType) : IRequest<AddressType?>;

    public class UpdateAddressTypeHandler : IRequestHandler<UpdateAddressTypeCommand, AddressType?>
    {
        private readonly IRepository<AddressType> _addressTypeRepository;
        private readonly IValidator<AddressType> _validator;
        private readonly IMapper _mapper;

        public UpdateAddressTypeHandler(
            IRepository<AddressType> addressTypeRepository,
            IValidator<AddressType> validator,
            IMapper mapper
        )
        {
            _addressTypeRepository = addressTypeRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<AddressType?> Handle(
            UpdateAddressTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            var updatedAddressType = request.AddressType;
            var addressType = await _addressTypeRepository.GetByIdAsync(updatedAddressType.Id);
            if (addressType == null)
                return null;

            _mapper.Map(updatedAddressType, addressType);

            var validationResult = await _validator.ValidateAsync(addressType, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _addressTypeRepository.Update(addressType);
            await _addressTypeRepository.SaveChangesAsync();
            return addressType;
        }
    }
}
