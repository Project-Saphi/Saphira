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

            var content = new List<string>();
            var playerCount = 0;

            foreach (var leaderboardEntry in result.Response.Data)
            {
                content.Add($"#{leaderboardEntry.Rank} - {MessageTextFormat.Bold(leaderboardEntry.Holder)} - {ScoreFormatter.AsIngameTime(leaderboardEntry.MinScore)}");
                playerCount++;

                if (playerCount > (_configuration.MaxLeaderboardEntries - 1))
                {
                    break;
                }
            }

            var embed = new EmbedBuilder();
            
            var field = new EmbedFieldBuilder();
            field.WithName(MessageTextFormat.Bold($"Top {_configuration.MaxLeaderboardEntries} {categoryEntity?.Name ?? category} times on {customTrackEntity?.Name ?? customTrack}"));
            field.WithValue(String.Join("\n", content));

            embed.AddField(field);

            await RespondAsync(embed: embed.Build());
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
