// ai-addresses-services/Queries/GetPersonByIdQuery.cs
using MediatR;
using ai_addresses_data.Entity;
using MediatR;
using ai_addresses_data.Repository;



namespace ai_addresses_services.Queries
{
    public record GetPersonByIdQuery(int Id) : IRequest<Person?>;

    public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, Person?>
    {
        private readonly IRepository<Person> _personRepository;

        public GetPersonByIdHandler(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<Person?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            return await _personRepository.GetByIdAsync(request.Id);
        }
    } 
}