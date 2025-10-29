using Discord;
using Discord.WebSocket;

namespace Saphira.Discord.EventSubscriber
{
    public class MessageReceivedEventSubscriber : IDiscordEventSubscriber
    {
        private readonly DiscordSocketClient _client;

        private bool _isRegistered = false;

        public MessageReceivedEventSubscriber(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Register()
        {
            if (_isRegistered) return;

            _client.MessageReceived += HandleMessageReceivedAsync;
            _isRegistered = true;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            _client.MessageReceived -= HandleMessageReceivedAsync;
            _isRegistered = false;
        }

        private Task HandleMessageReceivedAsync(SocketMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
