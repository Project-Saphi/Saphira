using Discord;
using Discord.WebSocket;
using Saphira.Core.Application;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Logging;

namespace Saphira.Discord.Event;

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

    private async Task HandleReadyAsync()
    {
        Application.StartTime = DateTime.UtcNow;

        logger.Log(LogSeverity.Info, "Saphira", "Connection to Discord established");
        logger.Log(LogSeverity.Info, "Saphira", "Saphira started successfully");
    }
}
