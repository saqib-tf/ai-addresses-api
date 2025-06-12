using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Commands
{
    public record DeleteStateCommand(int Id) : IRequest<bool>;

    public class DeleteStateHandler : IRequestHandler<DeleteStateCommand, bool>
    {
        private readonly IRepository<State> _stateRepository;

        public DeleteStateHandler(IRepository<State> stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<bool> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _stateRepository.GetByIdAsync(request.Id);
            if (state == null)
                return false;

            _stateRepository.Remove(state);
            await _stateRepository.SaveChangesAsync();
            return true;
        }
    }
}