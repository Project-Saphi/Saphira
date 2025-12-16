using Discord;
using Discord.WebSocket;
using Saphira.Discord.Pagination.Component;

namespace Saphira.Discord.Pagination.Builder;

public abstract class PaginationBuilderBase<T, TBuilder> where TBuilder : PaginationBuilderBase<T, TBuilder>
{
    protected readonly PaginationComponentHandler _paginationComponentHandler;
    protected int _pageSize = 10;
    protected Guid _customId = Guid.NewGuid();
    protected Func<SocketMessageComponent, Task<PaginationFilterResult>>? _filter;

    protected PaginationBuilderBase(PaginationComponentHandler paginationComponentHandler)
    {
        _paginationComponentHandler = paginationComponentHandler;
    }

    public TBuilder WithPageSize(int pageSize)
    {
        _pageSize = pageSize;
        return (TBuilder)this;
    }

    public TBuilder WithCustomId(Guid customId)
    {
        _customId = customId;
        return (TBuilder)this;
    }

    public TBuilder WithFilter(Func<SocketMessageComponent, Task<PaginationFilterResult>> filter)
    {
        _filter = filter;
        return (TBuilder)this;
    }

    protected (Embed embed, MessageComponent components) BuildPageResult(EmbedBuilder embedBuilder, Pagination pagination)
    {
        var components = new PaginationComponentBuilder(
            pagination.ID,
            pagination.IsFirstPage(),
            pagination.IsLastPage()
        );

        return (embedBuilder.Build(), components.Build());
    }

    protected void RegisterPaginationState(Pagination pagination, Func<SocketMessageComponent, Pagination, Task> updateCallback)
    {
        var state = new PaginationState(pagination, updateCallback, _filter);
        _paginationComponentHandler.RegisterPagination(pagination.ID, state);
    }
}
