using Discord;

namespace Saphira.Discord.Messaging.Pagination;

public class PaginationComponentBuilder : ComponentBuilder
{
    public PaginationComponentBuilder(Guid paginationId, bool disablePrevious = false, bool disableNext = false)
    {
        var previousPageButtonBuilder = new ButtonBuilder()
            .WithLabel("Previous")
            .WithStyle(ButtonStyle.Secondary)
            .WithEmote(new Emoji("⬅️"))
            .WithCustomId($"pagination:prev:{paginationId}")
            .WithDisabled(disablePrevious);

        var nextPageButtonBuilder = new ButtonBuilder()
            .WithLabel("Next")
            .WithStyle(ButtonStyle.Secondary)
            .WithEmote(new Emoji("➡️"))
            .WithCustomId($"pagination:next:{paginationId}")
            .WithDisabled(disableNext);

        WithButton(previousPageButtonBuilder);
        WithButton(nextPageButtonBuilder);
    }

}
