namespace Saphira.Discord.Pagination;

public record PaginationFilterResult(
    bool Success,
    string? CustomErrorMessage = null
);
