using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Discord.Guild;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.Pagination;
using Saphira.Util.EmoteMapper;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LivestreamsCommand(PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    public readonly int EntriesPerPage = 10;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/livestreams",
            "This command can only detect streams from users whose Discord status is set to `Streaming`"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("livestreams", "List all CTR livestreams from server members")]
    public async Task HandleCommand()
    {
        var activities = GuildManager.FindCTRStreamActivities(Context.Guild);

        if (activities.Count == 0)
        {
            var infoAlert = new InfoAlertEmbedBuilder("There is currently nobody streaming Crash Team Racing.");
            await RespondAsync(embed: infoAlert.Build());
            return;
        }

        var paginationBuilder = new PaginationBuilder<(SocketGuildUser User, IActivity Activity)>(paginationComponentHandler)
            .WithItems(activities)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageEntries, pageNumber) => GetEmbedForPage(pageEntries, pageNumber));

        var (embed, components) = paginationBuilder.Build();

        await RespondAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(List<(SocketGuildUser User, IActivity Activity)> activities, int page)
    {
        var activityData = GetActivityData(activities);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {page}] CTR Livestreams");

        AddEmbedField(embed, ":video_camera:", "Streamer", activityData["streamers"]);
        AddEmbedField(embed, ":desktop:", "Platform", activityData["platforms"]);
        AddEmbedField(embed, ":label:", "Title", activityData["titles"]);

        return embed;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private Dictionary<string, List<string>> GetActivityData(List<(SocketGuildUser, IActivity Activity)> activities)
    {
        var dict = new Dictionary<string, List<string>>()
        {
            {
                "streamers", []
            },
            {
                "platforms", []
            },
            {
                "titles", []
            }
        };

        foreach (var (user, activity) in activities)
        {
            if (activity is StreamingGame stream)
            {
                dict["streamers"].Add($"{user.Mention}");
                dict["platforms"].Add($"{StreamingPlatformEmoteMapper.MapStreamingPlatformToEmote(activity.Name)} [{activity.Name}]({stream.Url})");
                dict["titles"].Add($"```{stream.Details}```");
            }
        }

        return dict;
    }
}
