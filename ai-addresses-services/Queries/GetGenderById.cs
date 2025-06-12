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
    public record GetGenderByIdQuery(int Id) : IRequest<Gender?>;

    // Handler
    public class GetGenderByIdHandler : IRequestHandler<GetGenderByIdQuery, Gender?>
    {
        private readonly IRepository<Gender> _genderRepository;

        public GetGenderByIdHandler(IRepository<Gender> genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public async Task<Gender?> Handle(GetGenderByIdQuery request, CancellationToken cancellationToken)
        {
            return await _genderRepository.GetByIdAsync(request.Id);
        }
    }
}
