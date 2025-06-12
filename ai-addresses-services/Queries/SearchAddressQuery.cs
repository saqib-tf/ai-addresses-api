using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using ai_addresses_services.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ai_addresses_services.Queries
{
    public record SearchAddressQuery(PagedQuery Query, int? PersonId = null)
        : IRequest<PagedResult<Address>>;

    public class SearchAddressHandler : IRequestHandler<SearchAddressQuery, PagedResult<Address>>
    {
        private readonly IRepository<Address> _repo;
        private readonly IValidator<SearchAddressQuery> _validator;

        public SearchAddressHandler(
            IRepository<Address> repo,
            IValidator<SearchAddressQuery> validator
        )
        {
            _repo = repo;
            _validator = validator;
        }

        public async Task<PagedResult<Address>> Handle(
            SearchAddressQuery request,
            CancellationToken cancellationToken
        )
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var q = request.Query;
            var searchTerm = q.SearchTerm?.ToLower();

            // Build predicate for filtering
            var addresses = await _repo.FindAsync(
                x =>
                    x.PersonId == request.PersonId
                    && (
                        string.IsNullOrWhiteSpace(searchTerm)
                        || x.Street.ToLower().Contains(searchTerm)
                        || x.City.ToLower().Contains(searchTerm)
                        || (x.PostalCode != null && x.PostalCode.ToLower().Contains(searchTerm))
                    ),
                query =>
                    query
                        .Include(a => a.State)
                        .ThenInclude(s => s.Country)
                        .Include(a => a.AddressType)
            );

            var filtered = addresses.AsQueryable();

            filtered = q.SortBy?.ToLower() switch
            {
                "street" => q.SortDescending
                    ? filtered.OrderByDescending(x => x.Street)
                    : filtered.OrderBy(x => x.Street),
                "city" => q.SortDescending
                    ? filtered.OrderByDescending(x => x.City)
                    : filtered.OrderBy(x => x.City),
                "postalcode" => q.SortDescending
                    ? filtered.OrderByDescending(x => x.PostalCode)
                    : filtered.OrderBy(x => x.PostalCode),
                _ => filtered.OrderBy(x => x.Id),
            };

            var total = filtered.Count();
            var items = filtered.Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToList();

            return new PagedResult<Address>(items, total, q.PageNumber, q.PageSize);
        }
    }
}
