using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Autocompletion;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;
using Saphira.Util.Mapper;

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
        [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string track,
        [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
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

        var initialPagination = new Pagination(1, EntriesPerPage, leaderboardEntries.Count);
        var paginationId = Guid.NewGuid();
        var paginationComponentBuilder = new PaginationComponentBuilder(paginationId, disablePrevious: initialPagination.IsFirstPage(), disableNext: initialPagination.IsLastPage());

        var firstPageEntries = leaderboardEntries.Take(initialPagination.GetLimit()).ToList();
        var firstPageEmbed = GetEmbedForPage(customTrack, firstPageEntries, initialPagination.CurrentPage);

        RegisterPagination(paginationId, initialPagination, leaderboardEntries, customTrack);

        await RespondAsync(embed: firstPageEmbed.Build(), components: paginationComponentBuilder.Build());
    }

    private void RegisterPagination(Guid paginationId, Pagination pagination, List<TrackLeaderboardEntry> leaderboardEntries, CustomTrack track)
    {
        var state = new PaginationState(pagination, async (component, newPagination) =>
        {
            var pageEntries = leaderboardEntries.Skip(newPagination.GetOffset()).Take(newPagination.GetLimit()).ToList();
            var embed = GetEmbedForPage(track, pageEntries, newPagination.CurrentPage);
            var updatedComponents = new PaginationComponentBuilder(paginationId, newPagination.IsFirstPage(), newPagination.IsLastPage());

            await component.UpdateAsync(msg =>
            {
                msg.Embed = embed.Build();
                msg.Components = updatedComponents.Build();
            });
        });

        paginationComponentHandler.RegisterPagination(paginationId, state);
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
