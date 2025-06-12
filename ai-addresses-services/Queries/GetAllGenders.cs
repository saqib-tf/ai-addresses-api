using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    // Query
    public record GetAllGendersQuery() : IRequest<IEnumerable<Gender>>;

    // Handler
    public class GetAllGendersHandler : IRequestHandler<GetAllGendersQuery, IEnumerable<Gender>>
    {
        private readonly IRepository<Gender> _genderRepository;

        public GetAllGendersHandler(IRepository<Gender> genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public async Task<IEnumerable<Gender>> Handle(GetAllGendersQuery request, CancellationToken cancellationToken)
        {
            return await _genderRepository.GetAllAsync();
        }
    }
}
