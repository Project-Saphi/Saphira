using Discord;
using Saphira.Discord.Pagination.Component;

namespace Saphira.Discord.Pagination.Builder;

public class CallbackPaginationBuilder<T> : PaginationBuilderBase<T, CallbackPaginationBuilder<T>>
{
    private int _totalItems = 0;
    private Func<int, int, Task<List<T>>>? _fetchCallback;
    private Func<List<T>, int, int, EmbedBuilder>? _renderPageCallback;

    public CallbackPaginationBuilder(PaginationComponentHandler paginationComponentHandler)
        : base(paginationComponentHandler)
    {
    }

    public CallbackPaginationBuilder<T> WithTotalItems(int totalItems)
    {
        _totalItems = totalItems;
        return this;
    }

    public CallbackPaginationBuilder<T> WithFetchCallback(Func<int, int, Task<List<T>>> fetchCallback)
    {
        _fetchCallback = fetchCallback;
        return this;
    }

    public CallbackPaginationBuilder<T> WithRenderPageCallback(Func<List<T>, int, int, EmbedBuilder> renderPageCallback)
    {
        _renderPageCallback = renderPageCallback;
        return this;
    }

    public async Task<(Embed embed, MessageComponent components)> BuildAsync()
    {
        if (_fetchCallback == null)
        {
            throw new InvalidOperationException("FetchCallback must be set before building.");
        }

        var initialPagination = new Pagination(_customId, 1, _pageSize, _totalItems);

        RegisterPaginationState(initialPagination, async (component, newPagination) =>
        {
            var (embed, components) = await GetPageAsync(newPagination);

            await component.UpdateAsync(message =>
            {
                message.Embed = embed;
                message.Components = components;
            });
        });

        return await GetPageAsync(initialPagination);
    }

    private async Task<(Embed embed, MessageComponent components)> GetPageAsync(Pagination pagination)
    {
        var pageItems = await _fetchCallback!(pagination.CurrentPage, pagination.PageSize);

        var embed = _renderPageCallback != null
            ? _renderPageCallback(pageItems, pagination.CurrentPage, (int)pagination.GetPageCount())
            : new EmbedBuilder();

        return BuildPageResult(embed, pagination);
    }
}
