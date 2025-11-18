using Discord.WebSocket;
using Saphira.Discord.Messaging.Pagination;
using Saphira.Extensions.DependencyInjection;

namespace Saphira.Discord.EventSubscriber;

[AutoRegister]
public class ButtonExecutedEventSubscriber(DiscordSocketClient client, PaginationComponentHandler paginationComponentHandler) : IDiscordSocketClientEventSubscriber
{
    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.ButtonExecuted += HandleButtonExecutedAsync;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.ButtonExecuted -= HandleButtonExecutedAsync;
        _isRegistered = false;
    }

    private async Task HandleButtonExecutedAsync(SocketMessageComponent component)
    {
        try
        {
            await paginationComponentHandler.HandleComponentInteraction(component);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling button interaction: {ex}");

            // Try to respond to the interaction to prevent "This interaction failed" message
            try
            {
                if (!component.HasResponded)
                {
                    await component.RespondAsync("An error occurred while processing your request.", ephemeral: true);
                }
            }
            catch
            {
                // Ignore if we can't respond
            }
        }
    }
}
