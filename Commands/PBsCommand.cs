using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Util.Game;

namespace Saphira.Commands
{
    public class PBsCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Client _client;

        public PBsCommand(Client client)
        {
            _client = client;
        }

        [RequireTextChannel]
        [SlashCommand("pbs", "Get personal best times of a player")]
        public async Task HandleCommand(string player)
        {
            var result = await _client.GetPlayerPBsAsync(player);

            if (result.Success == false || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"personal best times of {player}: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var content = new List<string>();
            var count = 1;

            foreach (var pbEntry in result.Response.Data)
            {
                content.Add($"{count}. {MessageTextFormat.Bold(pbEntry.TrackName)} - {MessageTextFormat.Bold(ScoreFormatter.AsIngameTime(pbEntry.Time))} (Rank {pbEntry.Rank})");
                count++;
            }

            var embed = new EmbedBuilder();
            var field = new EmbedFieldBuilder();
            field.WithName(MessageTextFormat.Bold($"Personal best times of {player}"));
            field.WithValue(String.Join("\n", content));
            embed.AddField(field);

            await RespondAsync(embed: embed.Build());
        }
    }
}
