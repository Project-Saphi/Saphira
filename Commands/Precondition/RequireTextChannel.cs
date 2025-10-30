using Discord.Commands;
using Discord.WebSocket;

namespace Saphira.Commands.Precondition
{
    public class RequireTextChannel : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, 
            CommandInfo command, 
            IServiceProvider services
            )
        {
            var textChannel = context.Channel as SocketTextChannel;

            if (textChannel != null)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("This command can only be used in a text channel."));
        }
    }
}
