using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Discord.Guild;

namespace Saphira.Discord.Interaction.Precondition;

public class RequireTeamMemberRole : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services
        )
    {
        if (context.User is not SocketGuildUser socketGuildUser)
        {
            return Task.FromResult(PreconditionResult.FromError("User not found."));
        }

        if (socketGuildUser.Roles.Any(role => GuildRole.IsTeamRole(role)))
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        return Task.FromResult(PreconditionResult.FromError("You don't have permission to use this command."));
      }
}
