using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Discord.Guild;

namespace Saphira.Commands.Precondition
{
    public class RequireCommandAllowedChannel : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(
            IInteractionContext context,
            ICommandInfo commandInfo,
            IServiceProvider services
            )
        {
            if (context.Channel is not SocketTextChannel textChannel)
            {
                return Task.FromResult(PreconditionResult.FromError("The channel does not exist."));
            }

            if (context.User is not SocketUser user)
            {
                return Task.FromResult(PreconditionResult.FromError("The user does not exist."));
            }

            var configuration = services.GetRequiredService<Configuration>();
            if (configuration.CommandsAllowedChannels.Any(channel => channel == textChannel.Name) || GuildMember.IsTeamMember(user))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("You cannot use commands in this channel."));
        }
    }
}
