using Discord.WebSocket;

namespace Saphira.Discord.Pagination;

public record PaginationState(
    Pagination Pagination,
    Func<SocketMessageComponent, Pagination, Task> UpdateCallback
);
