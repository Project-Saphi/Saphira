using Discord;
using Discord.Interactions;
using Saphira.Discord.Core.Interaction.Autocompletion;
using Saphira.Discord.Core.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Discord.Pagination;
using Saphira.Discord.Pagination.Builder;
using Saphira.Discord.Pagination.Component;
using Saphira.Saphi.Api;
using Saphira.Saphi.Core.Entity;
using Saphira.Saphi.Core.Entity.Leaderboard;
using Saphira.Saphi.Core;
using Saphira.Saphi.Interaction.Autocompletion.ValueProvider;
using System.Text;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LeaderboardCommand(ISaphiApiClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
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

        var customTrackResult = await client.GetCustomTrackAsync(track);

        if (!customTrackResult.Success || customTrackResult.Response == null || customTrackResult.Response.Data.Count == 0)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"There is no custom track with id {track}.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var customTrack = customTrackResult.Response.Data.First();

        // Fetch initial page to check if there's any data and get total count
        var initialResult = await client.GetTrackLeaderboardAsync(track, category, page: 1, perPage: EntriesPerPage);

        if (!initialResult.Success || initialResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve leaderboard: {initialResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (initialResult.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder($"Nobody has set a time on {customTrack.Name} yet.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var totalItems = initialResult.Response.Meta.Total;

        var paginationBuilder = new CallbackPaginationBuilder<TrackLeaderboardEntry>(paginationComponentHandler)
            .WithPageSize(EntriesPerPage)
            .WithTotalItems(totalItems)
            .WithFetchCallback(async (page, perPage) =>
            {
                var result = await client.GetTrackLeaderboardAsync(track, category, page: page, perPage: perPage);
                return result.Response?.Data ?? [];
            })
            .WithRenderPageCallback((pageEntries, pageNumber, totalPages) => GetEmbedForPage(customTrack, pageEntries, pageNumber, totalPages))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = await paginationBuilder.BuildAsync();

        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(CustomTrack customTrack, List<TrackLeaderboardEntry> leaderboardEntries, int currentPage, int totalPages)
    {
        if (leaderboardEntries.Count == 0)
        {
            return new EmbedBuilder().WithDescription("No entries found.");
        }

        var firstEntry = leaderboardEntries.First();
        var leaderboardData = GetLeaderboardData(leaderboardEntries);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {currentPage}/{totalPages}] Leaderboard for {customTrack.Name} ({firstEntry.CategoryName})");

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
            { "placements", [] },
            { "players", [] },
            { "times", [] }
        };

        foreach (var entry in entries)
        {
            dict["placements"].Add(BuildPlacementString(entry));
            dict["players"].Add(BuildPlayerString(entry));
            dict["times"].Add(BuildTimeString(entry));
        }

        return dict;
    }

    private string BuildPlacementString(TrackLeaderboardEntry entry)
    {
        var standardEmote = entry.StandardId.HasValue
            ? TierEmoteMapper.MapTierToEmote(entry.StandardId.Value.ToString())
            : null;

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
            .Append(MessageTextFormat.Bold(entry.Name))
            .ToString();
    }

    private string BuildTimeString(TrackLeaderboardEntry entry)
    {
        return new StringBuilder()
            .Append(CharacterEmoteMapper.MapCharacterToEmote(entry.CharacterName))
            .Append(' ')
            .Append(entry.TimeFormatted)
            .ToString();
    }
}
