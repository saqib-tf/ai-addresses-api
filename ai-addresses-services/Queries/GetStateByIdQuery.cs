using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record GetStateByIdQuery(int Id) : IRequest<State?>;

    public class GetStateByIdHandler : IRequestHandler<GetStateByIdQuery, State?>
    {
        private readonly IRepository<State> _stateRepository;

        public GetStateByIdHandler(IRepository<State> stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<State?> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _stateRepository.GetByIdAsync(request.Id);
        }
    }
}