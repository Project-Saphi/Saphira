using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Foundation.Autocompletion;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Discord.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Saphi.Game;
using Saphira.Saphi.Interaction;
using System.Text;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LeaderboardCommand(ISaphiApiClient client, StandardCalculator standardCalculator, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 15;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/leaderboard Frozen Depths Course",
            $"{EntriesPerPage} entries are shown per page"
        );
    }

    [SlashCommand("leaderboard", "Get the leaderboard for a single track and category")]
    public async Task HandleCommand(
        [Autocomplete(typeof(GenericAutocompleteHandler<CustomTrackValueProvider>))] int track,
        [Autocomplete(typeof(GenericAutocompleteHandler<CategoryValueProvider>))] int category
        )
    {
        await DeferAsync();

        var leaderboardResult = await client.GetTrackLeaderboardAsync(track, category);

        if (!leaderboardResult.Success || leaderboardResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve leaderboard: {leaderboardResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var customTrackResult = await client.GetCustomTrackAsync(track);

        if (!customTrackResult.Success || customTrackResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"There is no custom track with id {track}.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var customTrack = customTrackResult.Response.Data;

        if (leaderboardResult.Response.Data.Count == 0 || customTrackResult == null)
        {
            var warningAlert = new WarningAlertEmbedBuilder($"Nobody has set a time on {customTrack?.Name ?? "Unknown"} yet.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var leaderboardEntries = leaderboardResult.Response.Data;

        var paginationBuilder = new PaginationBuilder<TrackLeaderboardEntry>(paginationComponentHandler)
            .WithItems(leaderboardEntries)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageEntries, pageNumber) => GetEmbedForPage(customTrack, pageEntries, pageNumber))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();

        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(CustomTrack customTrack, List<TrackLeaderboardEntry> leaderboardEntries, int currentPage)
    {
        var firstEntry = leaderboardEntries.First();
        var leaderboardData = GetLeaderboardData(customTrack, leaderboardEntries);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {currentPage}] Leaderboard for {customTrack?.Name ?? "Unknown"} ({firstEntry?.CategoryName ?? "Unknown"})");

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

    private Dictionary<string, List<string>> GetLeaderboardData(CustomTrack customTrack, List<TrackLeaderboardEntry> entries)
    {
        var dict = new Dictionary<string, List<string>>()
        {
            { "placements", [] },
            { "players", [] },
            { "times", [] }
        };

        foreach (var entry in entries)
        {
            dict["placements"].Add(BuildPlacementString(customTrack, entry));
            dict["players"].Add(BuildPlayerString(entry));
            dict["times"].Add(BuildTimeString(entry));
        }

        return dict;
    }

    private string BuildPlacementString(CustomTrack customTrack, TrackLeaderboardEntry entry)
    {
        var standard = standardCalculator.CalculateStandard(customTrack, entry.CategoryId, entry.MinScore);
        var standardEmote = standard != null ? TierEmoteMapper.MapTierToEmote(standard.TierId.ToString()) : null;

        var placementString = new StringBuilder();

        if (standardEmote != null)
        {
            placementString
                .Append(standardEmote)
                .Append(' ');
        }

        return placementString
            .Append(RankFormatter.ToMedalFormat(entry.Rank))
            .ToString();
    }

    private string BuildPlayerString(TrackLeaderboardEntry entry)
    {
        return new StringBuilder()
            .Append(CountryEmoteMapper.MapCountryToEmote(entry.CountryName))
            .Append(' ')
            .Append(MessageTextFormat.Bold(entry.Holder))
            .ToString();
    }

    private string BuildTimeString(TrackLeaderboardEntry entry)
    {
        return new StringBuilder()
            .Append(CharacterEmoteMapper.MapCharacterToEmote(entry.CharacterName))
            .Append(' ')
            .Append(ScoreFormatter.AsHumanTime(entry.MinScore))
            .ToString();
    }
}
