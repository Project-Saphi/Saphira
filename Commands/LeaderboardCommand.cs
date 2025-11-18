using Discord;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Metadata;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;
using Saphira.Util.Mapper;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LeaderboardCommand(CachedClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    public override CommandMetadata GetMetadata()
    {
        return new CommandMetadata("/leaderboard Frozen Depths Course");
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

        var initialPagination = new Pagination(1, 10, leaderboardEntries.Count);
        var previousPageButtonId = Guid.NewGuid();
        var nextPageButtonId = Guid.NewGuid();
        var paginationComponentBuilder = new PaginationComponentBuilder(previousPageButtonId, nextPageButtonId, disablePrevious: initialPagination.IsFirstPage(), disableNext: initialPagination.IsLastPage());

        var firstPageEntries = leaderboardEntries.Take(initialPagination.GetLimit()).ToList();
        var firstPageEmbed = GetEmbedForPage(customTrack, firstPageEntries, initialPagination.CurrentPage);

        await RespondAsync(embed: firstPageEmbed.Build(), components: paginationComponentBuilder.Build());

        RegisterPaginationCallbacks(
            previousPageButtonId,
            nextPageButtonId,
            initialPagination,
            leaderboardEntries,
            customTrack
        );
    }

    private void RegisterPaginationCallbacks(Guid previousPageButtonId, Guid nextPageButtonId, Pagination pagination, List<TrackLeaderboardEntry> leaderboardEntries, CustomTrack track)
    {
        paginationComponentHandler.RegisterComponent(previousPageButtonId, pagination, async (component, p) =>
        {
            var newPagination = new Pagination(p.GetPreviousPage(), p.PageSize, p.ItemCount);
            var pageEntries = leaderboardEntries.Skip(newPagination.GetOffset()).Take(newPagination.GetLimit()).ToList();
            var embed = GetEmbedForPage(track, pageEntries, newPagination.CurrentPage);

            paginationComponentHandler.UpdatePagination(previousPageButtonId, newPagination);
            paginationComponentHandler.UpdatePagination(nextPageButtonId, newPagination);

            var updatedComponents = new PaginationComponentBuilder(previousPageButtonId, nextPageButtonId, newPagination.IsFirstPage(), newPagination.IsLastPage());

            await component.UpdateAsync(msg =>
            {
                msg.Embed = embed.Build();
                msg.Components = updatedComponents.Build();
            });
        });

        paginationComponentHandler.RegisterComponent(nextPageButtonId, pagination, async (component, p) =>
        {
            var newPagination = new Pagination(p.GetNextPage(), p.PageSize, p.ItemCount);
            var pageEntries = leaderboardEntries.Skip(newPagination.GetOffset()).Take(newPagination.GetLimit()).ToList();
            var embed = GetEmbedForPage(track, pageEntries, newPagination.CurrentPage);

            paginationComponentHandler.UpdatePagination(previousPageButtonId, newPagination);
            paginationComponentHandler.UpdatePagination(nextPageButtonId, newPagination);

            var updatedComponents = new PaginationComponentBuilder(previousPageButtonId, nextPageButtonId, newPagination.IsFirstPage(), newPagination.IsLastPage());

            await component.UpdateAsync(msg =>
            {
                msg.Embed = embed.Build();
                msg.Components = updatedComponents.Build();
            });
        });
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
