using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(30)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class TracksCommand(ISaphiApiClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 20;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/tracks", 
            $"{EntriesPerPage} entries are shown per page"
        );
    }

    [SlashCommand("tracks", "Get the list of supported custom tracks")]
    public async Task HandleCommand()
    {
        await DeferAsync();

        var result = await client.GetCustomTracksAsync();

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve custom track list: {result.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("There are no custom tracks supported yet.");
            await FollowupAsync(embed:  warningAlert.Build());
            return;
        }

        var customTracks = result.Response.Data;
        var submissionCounts = await CountSubmissions(customTracks);

        var paginationBuilder = new PaginationBuilder<CustomTrack>(paginationComponentHandler)
            .WithItems(customTracks)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageTracks, pageNumber) => GetEmbedForPage(pageTracks, pageNumber, submissionCounts))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();

        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(List<CustomTrack> pageTracks, int pageNumber, Dictionary<string, int> submissionCounts)
    {
        var customTracksData = GetCustomTracksData(pageTracks, submissionCounts);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}] Custom Track List");

        AddEmbedField(embed, ":identification_card:", "ID", customTracksData["ids"]);
        AddEmbedField(embed, ":motorway:", "Name", customTracksData["names"]);
        AddEmbedField(embed, ":art:", "Designer", customTracksData["creators"]);

        return embed;
    }

    private Dictionary<string, List<string>> GetCustomTracksData(List<CustomTrack> customTracks, Dictionary<string, int> submissionCounts)
    {
        var data = new Dictionary<string, List<string>>()
        {
            { "ids", [] },
            { "names", [] },
            { "creators", [] }
        };

        foreach (var track in customTracks)
        {
            data["ids"].Add($"#{track.Id}");
            data["names"].Add($"{MessageTextFormat.Bold(track.Name)} ({submissionCounts[track.Id]})");
            data["creators"].Add(track.Author);
        }

        return data;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private async Task<Dictionary<string, int>> CountSubmissions(List<CustomTrack> customTracks)
    {
        var count = new Dictionary<string, int>();

        var categoryResult = await client.GetCategoriesAsync();

        if (!categoryResult.Success || categoryResult.Response == null)
        {
            return count;
        }

        var categories = categoryResult.Response.Data;

        foreach (var category in categories)
        {
            foreach (var track in customTracks)
            {
                if (!count.ContainsKey(track.Id))
                {
                    count[track.Id] = 0;
                }

                var leaderboardResult = await client.GetTrackLeaderboardAsync(track.Id.ToString(), category.Id.ToString());

                if (!leaderboardResult.Success || leaderboardResult.Response == null)
                {
                    continue; 
                }

                count[track.Id] += leaderboardResult.Response.Data.Count;
            }
        }

        return count;
    }
}
