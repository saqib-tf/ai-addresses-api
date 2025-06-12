using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using ai_addresses_services.Models;
using MediatR;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Queries
{
    public record SearchStateQuery(PagedQuery Query, int? CountryId) : IRequest<PagedResult<State>>;

    public class SearchStateHandler : IRequestHandler<SearchStateQuery, PagedResult<State>>
    {
        private readonly IRepository<State> _repo;
        private readonly IValidator<SearchStateQuery> _validator;

        public SearchStateHandler(IRepository<State> repo, IValidator<SearchStateQuery> validator)
        {
            _repo = repo;
            _validator = validator;
        }

        public async Task<PagedResult<State>> Handle(SearchStateQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var q = request.Query;
            var all = await _repo.GetAllAsync();

            var query = all.AsQueryable();

            // Filter by CountryId if provided
            if (request.CountryId.HasValue)
            {
                query = query.Where(x => x.CountryId == request.CountryId.Value);
            }

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(q.SearchTerm))
            {
                var term = q.SearchTerm.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term) || x.Code.ToLower().Contains(term));
            }

            // Sort
            query = q.SortBy?.ToLower() switch
            {
                "name" => q.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "code" => q.SortDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
                _ => query.OrderBy(x => x.Id)
            };

            var total = query.Count();
            var items = query.Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToList();

            return new PagedResult<State>(items, total, q.PageNumber, q.PageSize);
        }
    }
}