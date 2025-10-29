using Discord;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Util.CTR;

namespace Saphira.Commands
{
    public class SaphiCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private SaphiApiClient _saphiApiClient;

        public SaphiCommands(SaphiApiClient saphiApiClient)
        {
            _saphiApiClient = saphiApiClient;
        }

        [RequireTextChannel]
        [SlashCommand("achievements", "Show a player's achievements")]
        public async Task AchievementsCommand(string player)
        {
            var response = await _saphiApiClient.GetUserProfileAsync(player);

            if (response?.Success == true)
            {
                var stats = response.Data.Stats;

                var lines = new List<string>();
                lines.Add($"{MessageTextFormat.Bold("Total Points")}: {stats.TotalPoints}");
                lines.Add($"{MessageTextFormat.Bold("Course Points")}: {stats.CoursePoints}");
                lines.Add($"{MessageTextFormat.Bold("Lap Points")}: {stats.LapPoints}");
                lines.Add($"{MessageTextFormat.Bold("First Places")}: {stats.FirstPlaces}");
                lines.Add($"{MessageTextFormat.Bold("Podium Finishes")}: {stats.PodiumFinishes}");

                var embed = new EmbedBuilder();
                var field = new EmbedFieldBuilder();
                field.WithName($"{response.Data.Username}'s Achievements");
                field.WithValue(String.Join("\n", lines));
                embed.AddField(field);

                await RespondAsync(embed: embed.Build());
            }
            else
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve user profile: {response?.Message}");
                await RespondAsync(embed: errorAlert.Build());
            }
        }

        [RequireTextChannel]
        [SlashCommand("tracks", "Get the list of supported custom tracks")]
        public async Task TracksCommand()
        {
            var response = await _saphiApiClient.GetCustomTracksAsync();

            if (response?.Success == true)
            {
                var lines = new List<string>();
                foreach (var customTrack in response.Data)
                {
                    lines.Add($"#{customTrack.Id} - {MessageTextFormat.Bold(customTrack.Name)}");
                }

                var embed = new EmbedBuilder();
                var field = new EmbedFieldBuilder();
                field.WithName(MessageTextFormat.Bold("Saphi Custom Track List"));
                field.WithValue(String.Join("\n", lines));
                embed.AddField(field);

                await RespondAsync(embed: embed.Build());
            }
            else
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve custom track list: {response?.Message}");
                await RespondAsync(embed: errorAlert.Build());
            }
        }

        [RequireTextChannel]
        [SlashCommand("pbs", "Get PBs of a player")]
        public async Task PBsCommand(string player)
        {
            var response = await _saphiApiClient.GetPlayerPBsAsync(player);

            if (response?.Success == true)
            {
                var lines = new List<string>();
                var count = 1;

                foreach (var pbEntry in response.Data)
                {
                    lines.Add($"{count}. {MessageTextFormat.Bold(pbEntry.TrackName)} - {MessageTextFormat.Bold(ScoreFormatter.AsIngameTime(pbEntry.Time))} (Rank {pbEntry.Rank})");
                    count++;
                }

                var embed = new EmbedBuilder();
                var field = new EmbedFieldBuilder();
                field.WithName(MessageTextFormat.Bold($"Personal bests of player \"{player}\""));
                field.WithValue(String.Join("\n", lines));
                embed.AddField(field);

                await RespondAsync(embed: embed.Build());
            }
            else
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve player PBs: {response?.Message}");
                await RespondAsync(embed: errorAlert.Build());
            }
        }

        [RequireTextChannel]
        [SlashCommand("leaderboard", "Get the leaderboard for a single track")]
        public async Task LeaderboardCommand(
            [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string track,
            [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
            )
        {
            var maxPlayers = 20;
            var response = await _saphiApiClient.GetTrackLeaderboardAsync(track, category);

            if (response?.Success == true)
            {
                var lines = new List<string>();
                var playerCount = 0;

                foreach (var leaderboardEntry in response.Data)
                {
                    lines.Add($"{leaderboardEntry.Rank}. {MessageTextFormat.Bold(leaderboardEntry.Holder)} - {MessageTextFormat.Bold(ScoreFormatter.AsIngameTime(leaderboardEntry.MinScore))}");
                    playerCount++;

                    if (playerCount > (maxPlayers - 1))
                    {
                        break;
                    }
                }

                var embed = new EmbedBuilder();
                var field = new EmbedFieldBuilder();
                field.WithName(MessageTextFormat.Bold($"Top {maxPlayers} times for category \"{category}\" on track \"{track}\""));
                field.WithValue(String.Join("\n", lines));
                embed.AddField(field);

                await RespondAsync(embed: embed.Build());
            }
            else
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve leaderboard: {response?.Message}");
                await RespondAsync(embed: errorAlert.Build());
            }
        }
    }
}
