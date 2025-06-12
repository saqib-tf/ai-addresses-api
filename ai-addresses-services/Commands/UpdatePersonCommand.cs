using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record UpdatePersonCommand(Person Person) : IRequest<Person?>;

    public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand, Person?>
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IValidator<Person> _validator;
        private readonly IMapper _mapper;

        public UpdatePersonHandler(
            IRepository<Person> personRepository,
            IValidator<Person> validator,
            IMapper mapper
        )
        {
            _personRepository = personRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<Person?> Handle(
            UpdatePersonCommand request,
            CancellationToken cancellationToken
        )
        {
            var updatedPerson = request.Person;
            var person = await _personRepository.GetByIdAsync(updatedPerson.Id);
            if (person == null)
                return null;

            _mapper.Map(updatedPerson, person);

            var validationResult = await _validator.ValidateAsync(person, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _personRepository.Update(person);
            await _personRepository.SaveChangesAsync();
            return person;
        }
    }
}
