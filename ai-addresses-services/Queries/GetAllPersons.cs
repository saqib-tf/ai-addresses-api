using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    // Query
    public record GetAllPersonsQuery() : IRequest<IEnumerable<Person>>;

    // Handler
    public class GetAllPersonsHandler : IRequestHandler<GetAllPersonsQuery, IEnumerable<Person>>
    {
        private readonly IRepository<Person> _personRepository;

        public GetAllPersonsHandler(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<Person>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
        {
            return await _personRepository.GetAllAsync();
        }
    }
}