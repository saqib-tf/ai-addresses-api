using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record CreateAddressCommand(Address Address) : IRequest<Address>;

    public class CreateAddressHandler : IRequestHandler<CreateAddressCommand, Address>
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IValidator<Address> _validator;

        public CreateAddressHandler(
            IRepository<Address> addressRepository,
            IValidator<Address> validator
        )
        {
            _addressRepository = addressRepository;
            _validator = validator;
        }

        public async Task<Address> Handle(
            CreateAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var address = request.Address;

            var validationResult = await _validator.ValidateAsync(address, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveChangesAsync();
            return address;
        }
    }
}
