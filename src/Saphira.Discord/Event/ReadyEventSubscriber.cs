using Discord;
using Discord.WebSocket;
using Saphira.Core;
using Saphira.Core.Event;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;

namespace Saphira.Discord.Event;

[AutoRegister]
public class ReadyEventSubscriber(Application application, DiscordSocketClient client, IMessageLogger logger) : IEventSubscriber
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
        application.StartTime = DateTime.UtcNow;

        logger.Log(LogSeverity.Info, "Saphira", "Connection to Discord established");
        logger.Log(LogSeverity.Info, "Saphira", "Saphira started successfully");
    }
}
