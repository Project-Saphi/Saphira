using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;

namespace Saphira.Commands
{
    public class PBsCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly CachedClient _client;

        public PBsCommand(CachedClient client)
        {
            _client = client;
        }

        [RequireTextChannel]
        [SlashCommand("pbs", "Get personal best times of a player")]
        public async Task HandleCommand(string player)
        {
            var result = await _client.GetPlayerPBsAsync(player);

            if (!result.Success || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve personal best times: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var content = new List<string>();
            foreach (var pbEntry in result.Response.Data)
            {
                var category = await FindCategory(pbEntry.CategoryId);
                content.Add($"- {MessageTextFormat.Bold(pbEntry.TrackName)} ({category?.Name ?? "Unknown"}) - {ScoreFormatter.AsIngameTime(pbEntry.Time)} (#{pbEntry.Rank})");
            }

            var playerName = result.Response.Data.First().Holder;
            var embed = new EmbedBuilder();

            var field = new EmbedFieldBuilder();
            field.WithName(MessageTextFormat.Bold($"Personal best times of {playerName}"));
            field.WithValue(String.Join("\n", content));

            embed.AddField(field);

            await RespondAsync(embed: embed.Build());
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
