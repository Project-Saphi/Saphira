using Discord;
using Discord.WebSocket;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Logging;

namespace Saphira.Discord.Event;

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
