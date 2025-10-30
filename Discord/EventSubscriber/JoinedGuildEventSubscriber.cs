using Discord.WebSocket;
using Saphira.Extensions.DependencyInjection;

namespace Saphira.Discord.EventSubscriber
{
    [AutoRegister]
    public class JoinedGuildEventSubscriber : IDiscordSocketClientEventSubscriber
    {
        private readonly DiscordSocketClient _client;
        private readonly Configuration _configuration;

        private bool _isRegistered = false;

        public JoinedGuildEventSubscriber(DiscordSocketClient client, Configuration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public void Register()
        {
            if (_isRegistered) return;

            _client.JoinedGuild += HandleGuildJoinedAsync;
            _isRegistered = true;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            _client.JoinedGuild -= HandleGuildJoinedAsync;
            _isRegistered = false;
        }

        private async Task HandleGuildJoinedAsync(SocketGuild guild)
        {
            var mainChannel = guild.Channels.FirstOrDefault(channel => channel.Name == _configuration.MainChannelName);

            if (mainChannel is not SocketTextChannel textChannel)
            {
                return;
            }

            await textChannel.SendMessageAsync("I am here. :slight_smile:");
            return;
        }
    }
}
