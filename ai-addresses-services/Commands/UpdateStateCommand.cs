using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace ai_addresses_services.Commands
{
    public record UpdateStateCommand(State State) : IRequest<State?>;

    public class UpdateStateHandler : IRequestHandler<UpdateStateCommand, State?>
    {
        private readonly IRepository<State> _stateRepository;
        private readonly IValidator<State> _validator;
        private readonly IMapper _mapper;

        public UpdateStateHandler(
            IRepository<State> stateRepository,
            IValidator<State> validator,
            IMapper mapper
        )
        {
            _stateRepository = stateRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<State?> Handle(
            UpdateStateCommand request,
            CancellationToken cancellationToken
        )
        {
            var updatedState = request.State;
            var state = await _stateRepository.GetByIdAsync(updatedState.Id);
            if (state == null)
                return null;

            _mapper.Map(updatedState, state);

            var validationResult = await _validator.ValidateAsync(state, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            _stateRepository.Update(state);
            await _stateRepository.SaveChangesAsync();
            return state;
        }
    }
}
