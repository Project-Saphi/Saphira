using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core.Event;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;

namespace Saphira.Discord.Interaction.Event;

[AutoRegister]
public class InteractionCreatedEventSubscriber(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider) : IEventSubscriber
{
    private readonly IMessageLogger _logger = serviceProvider.GetRequiredService<IMessageLogger>();
    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.InteractionCreated += HandleInteraction;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.InteractionCreated -= HandleInteraction;
        _isRegistered = false;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        await ExecuteCommandAsync(interaction);
    }

    private async Task ExecuteCommandAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(client, interaction);
            await interactionService.ExecuteCommandAsync(context, serviceProvider);
        }
        catch (Exception ex)
        {
            _logger.Log(LogSeverity.Error, "Saphira", $"Error handling interaction: {ex.Message}");

            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await InteractionResponder.RespondAsync(interaction, $"An error occurred executing the command. See console log for more information.");
            }
        }
    }
}
