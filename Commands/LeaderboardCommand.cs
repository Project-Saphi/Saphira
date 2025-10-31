using Discord;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    public class LeaderboardCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly CachedClient _client;
        private readonly Configuration _configuration;

        public LeaderboardCommand(CachedClient client, Configuration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        [SlashCommand("leaderboard", "Get the leaderboard for a single track (limited to 20 players)")]
        public async Task HandleCommand(
            [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string customTrack,
            [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
            )
        {
            var result = await _client.GetTrackLeaderboardAsync(customTrack, category);

            if (!result.Success || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve leaderboard: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var customTrackEntity = await FindCustomTrack(customTrack);
            var categoryEntity = await FindCategory(category);

            var embed = new EmbedBuilder();
            embed.WithAuthor($"Top {_configuration.MaxLeaderboardEntries} {categoryEntity?.Name ?? "Unknown"} times on {customTrackEntity?.Name ?? "Unknown"}");

            var placementsField = new EmbedFieldBuilder();
            placementsField.WithName($":trophy: {MessageTextFormat.Bold("Placement")}");
            placementsField.WithValue(String.Join("\n", GetPlacements(result.Response.Data)));
            placementsField.WithIsInline(true);

            var playersField = new EmbedFieldBuilder();
            playersField.WithName($":bust_in_silhouette: {MessageTextFormat.Bold("Player")}");
            playersField.WithValue(String.Join("\n", GetPlayers(result.Response.Data)));
            playersField.WithIsInline(true);

            var timesField = new EmbedFieldBuilder();
            timesField.WithName($":stopwatch: {MessageTextFormat.Bold("Time")}");
            timesField.WithValue(String.Join("\n", GetTimes(result.Response.Data)));
            timesField.WithIsInline(true);

            embed.AddField(placementsField);
            embed.AddField(playersField);
            embed.AddField(timesField);

            await RespondAsync(embed: embed.Build());
        }

        private List<string> GetPlacements(List<TrackLeaderboardEntry> leaderboardEntries)
        {
            var placements = new List<string>();

            foreach (var entry in leaderboardEntries)
            {
                placements.Add($"{RankFormatter.ToMedalFormat(entry.Rank)}");

                if (entry.Rank == _configuration.MaxLeaderboardEntries)
                {
                    break;
                }
            }

            return placements;
        }

        private List<string> GetPlayers(List<TrackLeaderboardEntry> leaderboardEntries)
        {
            var players = new List<string>();

            foreach (var entry in leaderboardEntries)
            {
                players.Add(MessageTextFormat.Bold(entry.Holder));

                if (entry.Rank == _configuration.MaxLeaderboardEntries)
                {
                    break;
                }
            }

            return players;
        }

        private List<string> GetTimes(List<TrackLeaderboardEntry> leaderboardEntries)
        {
            var times = new List<string>();

            foreach (var entry in leaderboardEntries)
            {
                times.Add(ScoreFormatter.AsIngameTime(entry.MinScore));

                if (entry.Rank == _configuration.MaxLeaderboardEntries)
                {
                    break;
                }
            }

            return times;
        }

        private async Task<CustomTrack?> FindCustomTrack(string trackId)
        {
            var result = await _client.GetCustomTracksAsync();

            if (!result.Success || result.Response == null)
            {
                return null;
            }

            return result.Response.Data.FirstOrDefault(customTrack => customTrack.Id == trackId);
        }

        private async Task<Category?> FindCategory(string categoryId)
        {
            var result = await _client.GetCategoriesAsync();

            if (!result.Success || result.Response == null)
            {
                return null;
            }

            return result.Response.Data.FirstOrDefault(category => category.Id == categoryId);
        }
    }
}
