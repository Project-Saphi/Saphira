using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Autocompletion;
using Saphira.Discord.Interaction.Autocompletion.ValueProvider;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.EmoteMapper;
using Saphira.Util.Game;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LeaderboardCommand(CachedClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 20;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/leaderboard Frozen Depths Course",
            $"{EntriesPerPage} entries are shown per page"
        );
    }

    [SlashCommand("leaderboard", "Get the leaderboard for a single track and category")]
    public async Task HandleCommand(
        [Autocomplete(typeof(GenericAutocompleteHandler<CustomTrackValueProvider>))] string track,
        [Autocomplete(typeof(GenericAutocompleteHandler<CategoryValueProvider>))] string category
        )
    {
        var result = await client.GetTrackLeaderboardAsync(track, category);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve leaderboard: {result.ErrorMessage ?? "Unknown error"}");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        var customTrack = await FindCustomTrack(track);

        if (result.Response.Data.Count == 0 || customTrack == null)
        {
            var warningAlert = new WarningAlertEmbedBuilder($"Nobody has set a time on {customTrack?.Name ?? "Unknown"} yet.");
            await RespondAsync(embed: warningAlert.Build());
            return;
        }

        var leaderboardEntries = result.Response.Data;

        var paginationBuilder = new PaginationBuilder<TrackLeaderboardEntry>(paginationComponentHandler)
            .WithItems(leaderboardEntries)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageEntries, pageNumber) => GetEmbedForPage(customTrack, pageEntries, pageNumber));

        var (embed, components) = paginationBuilder.Build();

        await RespondAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(CustomTrack track, List<TrackLeaderboardEntry> leaderboardEntries, int currentPage)
    {
        var firstEntry = leaderboardEntries.First();
        var leaderboardData = GetLeaderboardData(leaderboardEntries);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {currentPage}] Leaderboard for {track?.Name ?? "Unknown"} ({firstEntry?.CategoryName ?? "Unknown"})");

        AddEmbedField(embed, ":trophy:", "Placement", leaderboardData["placements"]);
        AddEmbedField(embed, ":bust_in_silhouette:", "Player", leaderboardData["players"]);
        AddEmbedField(embed, ":stopwatch:", "Time", leaderboardData["times"]);

        return embed;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private Dictionary<string, List<string>> GetLeaderboardData(List<TrackLeaderboardEntry> entries)
    {
        var dict = new Dictionary<string, List<string>>()
        {
            {
                "placements", []
            },
            {
                "players", []
            },
            {
                "times", []
            }
        };

        foreach (var entry in entries)
        {
            dict["placements"].Add(RankFormatter.ToMedalFormat(entry.Rank));
            dict["players"].Add($"{CountryEmoteMapper.MapCountryToEmote(entry.CountryName)} {MessageTextFormat.Bold(entry.Holder)}");
            dict["times"].Add($"{ScoreFormatter.AsHumanTime(entry.MinScore)} {CharacterEmoteMapper.MapCharacterToEmote(entry.CharacterName)}");
        }

        return dict;
    }

    private async Task<CustomTrack?> FindCustomTrack(string trackId)
    {
        var result = await client.GetCustomTracksAsync();

        if (!result.Success || result.Response == null)
        {
            return null;
        }

        return result.Response.Data.FirstOrDefault(customTrack => customTrack.Id == trackId);
    }
}
