using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Saphira.Discord.Interaction.Foundation.Precondition;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireTextChannel : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services
        )
    {
        if (context.Channel is SocketTextChannel)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        return Task.FromResult(PreconditionResult.FromError("This command can only be used in a text channel."));
    }
}
