namespace ai_addresses_services.Models
{
    public record PagedQuery(
        string? SearchTerm,
        string? SortBy,
        bool SortDescending,
        int PageNumber,
        int PageSize
    );
}