using Discord;

namespace Saphira.Discord.Messaging.Pagination;

public class PaginationBuilder<T>(PaginationComponentHandler paginationComponentHandler)
{
    private List<T> _items = [];
    private int _pageSize = 1;
    private Func<List<T>, int, EmbedBuilder>? _renderPageCallback;
    private Guid _customId = Guid.Empty;

    public PaginationBuilder<T> WithItems(List<T> items)
    {
        _items = items;
        return this;
    }

    public PaginationBuilder<T> WithPageSize(int pageSize)
    {
        _pageSize = pageSize; 
        return this; 
    }

    public PaginationBuilder<T> WithRenderPageCallback(Func<List<T>, int, EmbedBuilder> renderPageCallback)
    {
        _renderPageCallback = renderPageCallback;
        return this; 
    }

    public PaginationBuilder<T> WithCustomId(Guid customId)
    {
        _customId = customId;
        return this;
    }

    public (Embed embed, MessageComponent components) Build()
    {
        var initialPagination = new Pagination(_customId, 1, _pageSize, _items.Count);
        var state = new PaginationState(initialPagination, async (component, newPagination) =>
        {
            var (embed, components) = GetPage(newPagination);

            await component.UpdateAsync(message =>
            {
                message.Embed = embed;
                message.Components = components;
            });
        });

        paginationComponentHandler.RegisterPagination(initialPagination.ID, state);
        return GetPage(initialPagination);
    }

    private (Embed embed, MessageComponent components) GetPage(Pagination pagination)
    {
        var pageItems = _items
            .Skip(pagination.GetOffset())
            .Take(pagination.GetLimit())
            .ToList();

        // Default to empty embed when the render callback is not set
        // Shouldn't happen, but who knows?
        var embed = new EmbedBuilder();
        if (_renderPageCallback != null)
        {
            embed = _renderPageCallback(pageItems, pagination.CurrentPage);
        }

        var components = new PaginationComponentBuilder(
            pagination.ID,
            pagination.IsFirstPage(),
            pagination.IsLastPage()
        );

        return (embed.Build(), components.Build());
    }
}
