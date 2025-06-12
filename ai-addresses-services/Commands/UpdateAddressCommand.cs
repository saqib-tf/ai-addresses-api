using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record UpdateAddressCommand(Address Address) : IRequest<Address?>;

    public class UpdateAddressHandler : IRequestHandler<UpdateAddressCommand, Address?>
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IValidator<Address> _validator;
        private readonly IMapper _mapper;

        public UpdateAddressHandler(
            IRepository<Address> addressRepository,
            IValidator<Address> validator,
            IMapper mapper
        )
        {
            _addressRepository = addressRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<Address?> Handle(
            UpdateAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var updatedAddress = request.Address;
            var address = await _addressRepository.GetByIdAsync(updatedAddress.Id);
            if (address == null)
                return null;

            _mapper.Map(updatedAddress, address);

            var validationResult = await _validator.ValidateAsync(address, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();
            return address;
        }
    }
}
