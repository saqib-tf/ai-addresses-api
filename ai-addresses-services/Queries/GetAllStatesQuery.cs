using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record GetAllStatesQuery() : IRequest<IEnumerable<State>>;

    public class GetAllStatesHandler : IRequestHandler<GetAllStatesQuery, IEnumerable<State>>
    {
        private readonly IRepository<State> _stateRepository;

        public GetAllStatesHandler(IRepository<State> stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<IEnumerable<State>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
        {
            return await _stateRepository.GetAllAsync();
        }
    }
}