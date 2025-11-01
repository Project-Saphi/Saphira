using Discord;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LeaderboardCommand(CachedClient client, Configuration configuration) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("leaderboard", "Get the leaderboard for a single track (limited to 20 players)")]
    public async Task HandleCommand(
        [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string customTrack,
        [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
        )
    {
        var result = await client.GetTrackLeaderboardAsync(customTrack, category);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve leaderboard: {result.ErrorMessage ?? "Unknown error"}");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        var customTrackEntity = await FindCustomTrack(customTrack);

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder($"Nobody has set a time on {customTrackEntity?.Name ?? "Unknown"} yet.");
            await RespondAsync(embed: warningAlert.Build());
            return;
        }

        var firstEntry = result.Response.Data.First();

        var embed = new EmbedBuilder();
        embed.WithAuthor($"Top {configuration.MaxLeaderboardEntries} {firstEntry?.CategoryName ?? "Unknown"} times on {customTrackEntity?.Name ?? "Unknown"}");

        AddEmbedField(embed, ":trophy:", "Placement", GetPlacements(result.Response.Data));
        AddEmbedField(embed, ":bust_in_silhouette:", "Player", GetPlayers(result.Response.Data));
        AddEmbedField(embed, ":stopwatch:", "Time", GetTimes(result.Response.Data));

        await RespondAsync(embed: embed.Build());
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private List<string> GetPlacements(List<TrackLeaderboardEntry> entries) =>
        [.. entries.Take(configuration.MaxLeaderboardEntries).Select(e => RankFormatter.ToMedalFormat(e.Rank))];

    private List<string> GetPlayers(List<TrackLeaderboardEntry> entries) =>
        [.. entries.Take(configuration.MaxLeaderboardEntries).Select(e => MessageTextFormat.Bold(e.Holder))];

    private List<string> GetTimes(List<TrackLeaderboardEntry> entries) =>
        [.. entries.Take(configuration.MaxLeaderboardEntries).Select(e => ScoreFormatter.AsIngameTime(e.MinScore))];

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
