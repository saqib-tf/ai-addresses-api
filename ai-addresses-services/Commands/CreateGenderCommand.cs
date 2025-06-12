using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record CreateGenderCommand(Gender Gender) : IRequest<Gender>;

    public class CreateGenderHandler : IRequestHandler<CreateGenderCommand, Gender>
    {
        private readonly IRepository<Gender> _genderRepository;
        private readonly IValidator<Gender> _validator;

        public CreateGenderHandler(
            IRepository<Gender> genderRepository,
            IValidator<Gender> validator
        )
        {
            _genderRepository = genderRepository;
            _validator = validator;
        }

        public async Task<Gender> Handle(
            CreateGenderCommand request,
            CancellationToken cancellationToken
        )
        {
            var gender = request.Gender;

            var validationResult = await _validator.ValidateAsync(gender, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _genderRepository.AddAsync(gender);
            await _genderRepository.SaveChangesAsync();
            return gender;
        }
    }
}
