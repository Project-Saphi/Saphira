using Discord;
using Discord.WebSocket;
using Saphira.Util.Logging;

namespace Saphira.Discord.EventSubscriber
{
    public class ReadyEventSubscriber : IDiscordEventSubscriber
    {
        private readonly DiscordSocketClient _client;
        private readonly IMessageLogger _logger;

        private bool _isRegistered = false;

        public ReadyEventSubscriber(DiscordSocketClient client, IMessageLogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Register()
        {
            if (_isRegistered) return;

            _client.Ready += HandleReadyAsync;
            _isRegistered = true;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            _client.Ready -= HandleReadyAsync;
            _isRegistered = false;
        }

        private Task HandleReadyAsync()
        {
            Program.StartTime = DateTime.UtcNow;

            _logger.Log(LogSeverity.Info, "Saphira", "Connection to Discord established.");
            _logger.Log(LogSeverity.Info, "Saphira", "Slash commands are being registered ...");
            _logger.Log(LogSeverity.Info, "Saphira", "Saphira started successfully.");

            return Task.CompletedTask;
        }
    }
}
