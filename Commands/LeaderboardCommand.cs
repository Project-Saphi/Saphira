using Discord;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Util.Game;

namespace Saphira.Commands
{
    public class LeaderboardCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly CachedClient _client;
        private readonly Configuration _configuration;

        public LeaderboardCommand(CachedClient client, Configuration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        [RequireTextChannel]
        [SlashCommand("leaderboard", "Get the leaderboard for a single track (limited to 20 players)")]
        public async Task HandleCommand(
            [Autocomplete(typeof(CustomTrackAutocompleteHandler))] string track,
            [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
            )
        {
            var result = await _client.GetTrackLeaderboardAsync(track, category);

            if (result.Success == false || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve leaderboard for {track}: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var content = new List<string>();
            var playerCount = 0;

            foreach (var leaderboardEntry in result.Response.Data)
            {
                content.Add($"{leaderboardEntry.Rank}. {MessageTextFormat.Bold(leaderboardEntry.Holder)} - {MessageTextFormat.Bold(ScoreFormatter.AsIngameTime(leaderboardEntry.MinScore))}");
                playerCount++;

                if (playerCount > (_configuration.MaxLeaderboardEntries - 1))
                {
                    break;
                }
            }

            var embed = new EmbedBuilder();
            var field = new EmbedFieldBuilder();
            field.WithName(MessageTextFormat.Bold($"Top {_configuration.MaxLeaderboardEntries} {category} times on {track}"));
            field.WithValue(String.Join("\n", content));
            embed.AddField(field);

            await RespondAsync(embed: embed.Build());
        }
    }
}
