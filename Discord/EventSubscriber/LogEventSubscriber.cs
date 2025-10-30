using Discord;
using Discord.WebSocket;
using Saphira.Extensions.DependencyInjection;
using Saphira.Util.Logging;

namespace Saphira.Discord.EventSubscriber
{
    [AutoRegister]
    public class LogEventSubscriber : IDiscordSocketClientEventSubscriber
    {
        private readonly DiscordSocketClient _client;
        private readonly IMessageLogger _logger;

        private bool _isRegistered = false;

        public LogEventSubscriber(DiscordSocketClient client, IMessageLogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Register()
        {
            if (_isRegistered) return;

            _client.Log += HandleLogAsync;
            _isRegistered = true;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            _client.Log -= HandleLogAsync;
            _isRegistered = false;
        }

        private Task HandleLogAsync(LogMessage message)
        {
            _logger.Log(message);
            return Task.CompletedTask;
        }
    }
}
