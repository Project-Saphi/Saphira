using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Discord.Messaging.Pagination;

namespace Saphira.Discord.Interaction.Component;

public class PaginationInteraction(PaginationComponentHandler paginationComponentHandler) : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("pagination:prev:*")]
    public async Task HandlePreviousPage(string paginationId)
    {
        if (!Guid.TryParse(paginationId, out var guid))
        {
            await RespondAsync("Invalid pagination ID.", ephemeral: true);
            return;
        }

        await paginationComponentHandler.HandlePreviousPage(guid, (SocketMessageComponent) Context.Interaction);
    }

    [ComponentInteraction("pagination:next:*")]
    public async Task HandleNextPage(string paginationId)
    {
        if (!Guid.TryParse(paginationId, out var guid))
        {
            await RespondAsync("Invalid pagination ID.", ephemeral: true);
            return;
        }

        await paginationComponentHandler.HandleNextPage(guid, (SocketMessageComponent) Context.Interaction);
    }
}
