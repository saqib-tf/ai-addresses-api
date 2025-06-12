namespace ai_addresses_services.Models
{
    public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int PageNumber, int PageSize);
}