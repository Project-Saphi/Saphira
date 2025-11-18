using Discord;

namespace Saphira.Discord.Messaging.Pagination;

public class PaginationComponentBuilder : ComponentBuilder
{
    public readonly Guid PreviousPageButtonId;
    public readonly Guid NextPageButtonId;

    public PaginationComponentBuilder(Guid previousPageButtonId, Guid nextPageButtonId, bool disablePrevious = false, bool disableNext = false)
    {
        PreviousPageButtonId = previousPageButtonId;
        NextPageButtonId = nextPageButtonId;

        var previousPageButtonBuilder = new ButtonBuilder()
            .WithLabel("Previous")
            .WithStyle(ButtonStyle.Secondary)
            .WithEmote(new Emoji("⬅️"))
            .WithCustomId(previousPageButtonId.ToString())
            .WithDisabled(disablePrevious);

        var nextPageButtonBuilder = new ButtonBuilder()
            .WithLabel("Next")
            .WithStyle(ButtonStyle.Secondary)
            .WithEmote(new Emoji("➡️"))
            .WithCustomId(nextPageButtonId.ToString())
            .WithDisabled(disableNext);

        WithButton(previousPageButtonBuilder);
        WithButton(nextPageButtonBuilder);
    }

}
