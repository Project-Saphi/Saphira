using Discord;
using Discord.WebSocket;
using Saphira.Extensions.DependencyInjection;
using Saphira.Util.Logging;

namespace Saphira.Discord.EventSubscriber
{
    [AutoRegister]
    public class LogEventSubscriber(DiscordSocketClient client, IMessageLogger logger) : IDiscordSocketClientEventSubscriber
    {
        private bool _isRegistered = false;

        public void Register()
        {
            if (_isRegistered) return;

            client.Log += HandleLogAsync;
            _isRegistered = true;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            client.Log -= HandleLogAsync;
            _isRegistered = false;
        }

        private Task HandleLogAsync(LogMessage message)
        {
            logger.Log(message);
            return Task.CompletedTask;
        }
    }
}
