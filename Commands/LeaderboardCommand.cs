using Discord;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;
using Saphira.Util.Mapper;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LeaderboardCommand(CachedClient client, BotConfiguration botConfiguration) : InteractionModuleBase<SocketInteractionContext>
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

        var trackEntity = await FindCustomTrack(customTrack);

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder($"Nobody has set a time on {trackEntity?.Name ?? "Unknown"} yet.");
            await RespondAsync(embed: warningAlert.Build());
            return;
        }

        var firstEntry = result.Response.Data.First();
        var leaderboardData = GetLeaderboardData(result.Response.Data);

        var embed = new EmbedBuilder()
            .WithAuthor($"Top {botConfiguration.MaxLeaderboardEntries} {firstEntry?.CategoryName ?? "Unknown"} times on {trackEntity?.Name ?? "Unknown"}");

        AddEmbedField(embed, ":trophy:", "Placement", leaderboardData["placements"]);
        AddEmbedField(embed, ":bust_in_silhouette:", "Player", leaderboardData["players"]);
        AddEmbedField(embed, ":stopwatch:", "Time", leaderboardData["times"]);

        await RespondAsync(embed: embed.Build());
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

        foreach (var entry in entries.Take(botConfiguration.MaxLeaderboardEntries))
        {
            dict["placements"].Add(RankFormatter.ToMedalFormat(entry.Rank));
            dict["players"].Add($"{CountryEmoteMapper.MapCountryToEmote(entry.CountryName)} {MessageTextFormat.Bold(entry.Holder)}");
            dict["times"].Add($"{ScoreFormatter.AsIngameTime(entry.MinScore)} {CharacterEmoteMapper.MapCharacterToEmote(entry.CharacterName)}");
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
