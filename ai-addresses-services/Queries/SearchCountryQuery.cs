using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using ai_addresses_services.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record SearchCountryQuery(PagedQuery Query) : IRequest<PagedResult<Country>>;

    public class SearchCountryHandler : IRequestHandler<SearchCountryQuery, PagedResult<Country>>
    {
        private readonly IRepository<Country> _repo;

        public SearchCountryHandler(IRepository<Country> repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<Country>> Handle(SearchCountryQuery request, CancellationToken cancellationToken)
        {
            var q = request.Query;
            var all = await _repo.GetAllAsync();

            var query = all.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                var term = q.SearchTerm.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term) || x.Code.ToLower().Contains(term));
            }

            query = q.SortBy?.ToLower() switch
            {
                "name" => q.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "code" => q.SortDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
                _ => query.OrderBy(x => x.Id)
            };

            var total = query.Count();
            var items = query.Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToList();

            return new PagedResult<Country>(items, total, q.PageNumber, q.PageSize);
        }
    }
}