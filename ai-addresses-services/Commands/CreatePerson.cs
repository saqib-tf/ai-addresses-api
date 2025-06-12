using System;
using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record CreatePersonCommand(Person Person) : IRequest<Person>;

    public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, Person>
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IValidator<Person> _validator;

        public CreatePersonHandler(
            IRepository<Person> personRepository,
            IValidator<Person> validator
        )
        {
            _personRepository = personRepository;
            _validator = validator;
        }

        public async Task<Person> Handle(
            CreatePersonCommand request,
            CancellationToken cancellationToken
        )
        {
            var person = request.Person;

            var validationResult = await _validator.ValidateAsync(person, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();
            return person;
        }
    }
}
