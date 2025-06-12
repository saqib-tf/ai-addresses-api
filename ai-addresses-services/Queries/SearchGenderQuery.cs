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
    public record SearchGenderQuery(PagedQuery Query) : IRequest<PagedResult<Gender>>;

    public class SearchGenderHandler : IRequestHandler<SearchGenderQuery, PagedResult<Gender>>
    {
        private readonly IRepository<Gender> _genderRepository;

        public SearchGenderHandler(IRepository<Gender> genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public async Task<PagedResult<Gender>> Handle(SearchGenderQuery request, CancellationToken cancellationToken)
        {
            var q = request.Query;
            var all = await _genderRepository.GetAllAsync();

            // Filter
            var query = all.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                var term = q.SearchTerm.ToLower();
                query = query.Where(g =>
                    g.Name.ToLower().Contains(term) ||
                    g.Code.ToLower().Contains(term));
            }

            // Sort
            query = q.SortBy?.ToLower() switch
            {
                "name" => q.SortDescending ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
                "code" => q.SortDescending ? query.OrderByDescending(g => g.Code) : query.OrderBy(g => g.Code),
                _ => query.OrderBy(g => g.Id)
            };

            // Paging
            var total = query.Count();
            var items = query
                .Skip((q.PageNumber - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToList();

            return new PagedResult<Gender>(items, total, q.PageNumber, q.PageSize);
        }
    }
}