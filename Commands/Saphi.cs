using Discord;
using Discord.Interactions;
using Saphira.Discord;
using Saphira.Saphi.Api;
using Saphira.Util;

namespace Saphira.Commands
{
    public class Saphi : InteractionModuleBase<SocketInteractionContext>
    {
        private Client _client;

        public Saphi(Client client)
        {
            _client = client;
        }

        [SlashCommand("pbs", "Get PBs of a player")]
        public async Task PBsCommand(string playerId)
        {
            var queryParams = new Dictionary<string, string>
            {
                {
                    "player_id", playerId
                }
            };

            var response = await _client.GetAsync<GetPlayerPBsResponse>("player-pbs.php", queryParams);

            if (response?.Success == true)
            {
                var lines = new List<string>();
                var count = 1;

                foreach (var pbEntry in response.Data)
                {
                    lines.Add($"{count}. **{pbEntry.TrackName}** - **{ScoreFormatter.AsIngameTime(pbEntry.Time)}** (Rank {pbEntry.Rank})");
                    count++;
                }

                var embed = new EmbedBuilder();
                var field = new EmbedFieldBuilder();
                field.WithName($"Personal bests of player \"{playerId}\"");
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

        [SlashCommand("leaderboard", "Get the leaderboard for a single track")]
        public async Task LeaderboardCommand(string trackId, string categoryId)
        {
            var maxPlayers = 20;
            var queryParams = new Dictionary<string, string>
            {
                { "id", trackId },
                { "type", categoryId }
            };

            var response = await _client.GetAsync<GetTrackLeaderboardResponse>("track-leaderboards.php", queryParams);

            if (response?.Success == true)
            {
                var lines = new List<string>();
                var playerCount = 0;

                foreach (var leaderboardEntry in response.Data)
                {
                    lines.Add($"{leaderboardEntry.Rank}. **{leaderboardEntry.Holder}** - **{ScoreFormatter.AsIngameTime(leaderboardEntry.MinScore)}**");
                    playerCount++;

                    if (playerCount > (maxPlayers - 1))
                    {
                        break;
                    }
                }

                var embed = new EmbedBuilder();
                var field = new EmbedFieldBuilder();
                field.WithName($"Top {maxPlayers} times for category \"{categoryId}\" on track \"{trackId}\"");
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
