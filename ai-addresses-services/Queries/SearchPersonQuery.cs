using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using ai_addresses_services.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record SearchPersonQuery(PagedQuery Query) : IRequest<PagedResult<Person>>;

    public class SearchPersonHandler : IRequestHandler<SearchPersonQuery, PagedResult<Person>>
    {
        private readonly IRepository<Person> _repo;

        public SearchPersonHandler(IRepository<Person> repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<Person>> Handle(SearchPersonQuery request, CancellationToken cancellationToken)
        {
            var q = request.Query;

            // Use FindAsync with Include for Gender
            var all = await _repo.FindAsync(
                p => true,
                query => query.Include(x => x.Gender)
            );

            var filtered = all.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                var term = q.SearchTerm.ToLower();
                filtered = filtered.Where(x =>
                    x.FirstName.ToLower().Contains(term) ||
                    x.LastName.ToLower().Contains(term) ||
                    (x.Gender != null && x.Gender.Name.ToLower().Contains(term)) ||
                    (x.Gender != null && x.Gender.Code.ToLower().Contains(term))
                );
            }

            filtered = q.SortBy?.ToLower() switch
            {
                "firstname" => q.SortDescending ? filtered.OrderByDescending(x => x.FirstName) : filtered.OrderBy(x => x.FirstName),
                "lastname" => q.SortDescending ? filtered.OrderByDescending(x => x.LastName) : filtered.OrderBy(x => x.LastName),
                "gender" => q.SortDescending
                    ? filtered.OrderByDescending(x => x.Gender != null ? x.Gender.Name : null)
                    : filtered.OrderBy(x => x.Gender != null ? x.Gender.Name : null),
                _ => filtered.OrderBy(x => x.Id)
            };

            var total = filtered.Count();
            var items = filtered.Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToList();

            return new PagedResult<Person>(items, total, q.PageNumber, q.PageSize);
        }
    }
}