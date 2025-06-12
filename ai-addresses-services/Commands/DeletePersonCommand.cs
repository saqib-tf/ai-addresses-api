using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Commands
{
    public record DeletePersonCommand(int Id) : IRequest<bool>;

    public class DeletePersonHandler : IRequestHandler<DeletePersonCommand, bool>
    {
        private readonly IRepository<Person> _personRepository;

        public DeletePersonHandler(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<bool> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByIdAsync(request.Id);
            if (person == null)
                return false;

            _personRepository.Remove(person);
            await _personRepository.SaveChangesAsync();
            return true;
        }
    }
}