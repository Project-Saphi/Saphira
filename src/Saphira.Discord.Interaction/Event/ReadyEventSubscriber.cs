using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Core;
using Saphira.Core.Event;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;

namespace Saphira.Discord.Interaction.Event;

[AutoRegister]
public class ReadyEventSubscriber(DiscordSocketClient client, InteractionService interactionService, Configuration configuration, IMessageLogger logger) : IEventSubscriber
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
        await RegisterCommandsAsync();
    }

    private async Task RegisterCommandsAsync()
    {
        await interactionService.RegisterCommandsToGuildAsync(configuration.GuildId);
        logger.Log(LogSeverity.Info, "Saphira", $"Registered commands to guild {configuration.GuildId}");
    }
}
