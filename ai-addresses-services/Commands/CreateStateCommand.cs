using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record CreateStateCommand(State State) : IRequest<State>;

    public class CreateStateHandler : IRequestHandler<CreateStateCommand, State>
    {
        private readonly IRepository<State> _stateRepository;
        private readonly IValidator<State> _validator;

        public CreateStateHandler(IRepository<State> stateRepository, IValidator<State> validator)
        {
            _stateRepository = stateRepository;
            _validator = validator;
        }

        public async Task<State> Handle(
            CreateStateCommand request,
            CancellationToken cancellationToken
        )
        {
            var state = request.State;

            var validationResult = await _validator.ValidateAsync(state, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _stateRepository.AddAsync(state);
            await _stateRepository.SaveChangesAsync();
            return state;
        }
    }
}
