using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record UpdateGenderCommand(Gender Gender) : IRequest<Gender?>;

    public class UpdateGenderHandler : IRequestHandler<UpdateGenderCommand, Gender?>
    {
        private readonly IRepository<Gender> _genderRepository;
        private readonly IValidator<Gender> _validator;
        private readonly IMapper _mapper; // Add this

        public UpdateGenderHandler(
            IRepository<Gender> genderRepository,
            IValidator<Gender> validator,
            IMapper mapper // Add this
        )
        {
            _genderRepository = genderRepository;
            _validator = validator;
            _mapper = mapper; // Add this
        }

        public async Task<Gender?> Handle(
            UpdateGenderCommand request,
            CancellationToken cancellationToken
        )
        {
            var updatedGender = request.Gender;
            var gender = await _genderRepository.GetByIdAsync(updatedGender.Id);
            if (gender == null)
                return null;

            _mapper.Map(updatedGender, gender); // This copies all properties

            var validationResult = await _validator.ValidateAsync(gender, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _genderRepository.Update(gender);
            await _genderRepository.SaveChangesAsync();
            return gender;
        }
    }
}
