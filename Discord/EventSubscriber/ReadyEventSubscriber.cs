using Discord;
using Discord.WebSocket;
using Saphira.Extensions.DependencyInjection;
using Saphira.Util.Logging;

namespace Saphira.Discord.EventSubscriber;

[AutoRegister]
public class ReadyEventSubscriber(DiscordSocketClient client, IMessageLogger logger) : IDiscordSocketClientEventSubscriber
{
    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.Ready += HandleReadyAsync;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.Ready -= HandleReadyAsync;
        _isRegistered = false;
    }

    private Task HandleReadyAsync()
    {
        Program.StartTime = DateTime.UtcNow;

        logger.Log(LogSeverity.Info, "Saphira", "Connection to Discord established.");
        logger.Log(LogSeverity.Info, "Saphira", "Saphira started successfully.");

        return Task.CompletedTask;
    }
}
