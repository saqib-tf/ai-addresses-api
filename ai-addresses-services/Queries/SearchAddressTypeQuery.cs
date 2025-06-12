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
    public record SearchAddressTypeQuery(PagedQuery Query) : IRequest<PagedResult<AddressType>>;

    public class SearchAddressTypeHandler : IRequestHandler<SearchAddressTypeQuery, PagedResult<AddressType>>
    {
        private readonly IRepository<AddressType> _repo;

        public SearchAddressTypeHandler(IRepository<AddressType> repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<AddressType>> Handle(SearchAddressTypeQuery request, CancellationToken cancellationToken)
        {
            var q = request.Query;
            var all = await _repo.GetAllAsync();

            var query = all.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                var term = q.SearchTerm.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term));
            }

            query = q.SortBy?.ToLower() switch
            {
                "name" => q.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                _ => query.OrderBy(x => x.Id)
            };

            var total = query.Count();
            var items = query.Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToList();

            return new PagedResult<AddressType>(items, total, q.PageNumber, q.PageSize);
        }
    }
}