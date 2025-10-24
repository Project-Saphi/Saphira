using Discord.Interactions;

namespace Saphira.Commands
{
    public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Check the bot's latency")]
        public async Task PingCommand()
        {
            var latency = Context.Client.Latency;
            await RespondAsync($"🏓 Pong! Latency: {latency}ms");
        }

        [SlashCommand("server", "Get information about this server")]
        public async Task ServerInfoCommand()
        {
            var guild = Context.Guild;

            if (guild == null)
            {
                await RespondAsync("This command can only be used in a server!", ephemeral: true);
                return;
            }

            var response = $"**Server Name:** {guild.Name}\n" +
                          $"**Member Count:** {guild.MemberCount}\n" +
                          $"**Created:** {guild.CreatedAt:yyyy-MM-dd}\n" +
                          $"**Owner:** <@{guild.OwnerId}>";

            await RespondAsync(response);
        }
    }
}
