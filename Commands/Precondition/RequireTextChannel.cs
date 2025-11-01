using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Saphira.Commands.Precondition
{
    public class RequireTextChannel : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(
            IInteractionContext context,
            ICommandInfo commandInfo,
            IServiceProvider services
            )
        {
            if (context.Channel is SocketTextChannel textChannel)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("This command can only be used in a text channel."));
        }
    }
}
