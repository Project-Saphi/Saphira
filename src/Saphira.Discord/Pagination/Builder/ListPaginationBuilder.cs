using Discord;
using Saphira.Discord.Pagination.Component;

namespace Saphira.Discord.Pagination.Builder;

public class ListPaginationBuilder<T> : PaginationBuilderBase<T, ListPaginationBuilder<T>>
{
    private List<T> _items = [];
    private Func<List<T>, int, EmbedBuilder>? _renderPageCallback;

    public ListPaginationBuilder(PaginationComponentHandler paginationComponentHandler)
        : base(paginationComponentHandler)
    {
    }

    public ListPaginationBuilder<T> WithItems(List<T> items)
    {
        _items = items;
        return this;
    }

    public ListPaginationBuilder<T> WithRenderPageCallback(Func<List<T>, int, EmbedBuilder> renderPageCallback)
    {
        _renderPageCallback = renderPageCallback;
        return this;
    }

    public (Embed embed, MessageComponent components) Build()
    {
        var initialPagination = new Pagination(_customId, 1, _pageSize, _items.Count);

        RegisterPaginationState(initialPagination, async (component, newPagination) =>
        {
            var (embed, components) = GetPage(newPagination);

            await component.UpdateAsync(message =>
            {
                message.Embed = embed;
                message.Components = components;
            });
        });

        return GetPage(initialPagination);
    }

    private (Embed embed, MessageComponent components) GetPage(Pagination pagination)
    {
        var pageItems = _items
            .Skip(pagination.GetOffset())
            .Take(pagination.GetLimit())
            .ToList();

        var embed = _renderPageCallback != null
            ? _renderPageCallback(pageItems, pagination.CurrentPage)
            : new EmbedBuilder();

        return BuildPageResult(embed, pagination);
    }
}
