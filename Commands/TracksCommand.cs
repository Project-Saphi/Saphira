using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;

namespace Saphira.Commands
{
    public class TracksCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Client _client;

        public TracksCommand(Client client)
        {
            _client = client;
        }

        [RequireTextChannel]
        [SlashCommand("tracks", "Get the list of supported custom tracks")]
        public async Task HandleCommand()
        {
            var result = await _client.GetCustomTracksAsync();

            if (result.Success == false || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve custom track list: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var lines = new List<string>();
            foreach (var customTrack in result.Response.Data)
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
    }
}
