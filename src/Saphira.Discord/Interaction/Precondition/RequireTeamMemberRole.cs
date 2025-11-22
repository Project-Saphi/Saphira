using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Discord.Guild.Role;

namespace Saphira.Discord.Interaction.Precondition;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireTeamMemberRole : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services
        )
    {
        if (context.User is not SocketGuildUser guildUser)
        {
            return Task.FromResult(PreconditionResult.FromError("User not found."));
        }

        if (guildUser.Roles.Any(role => GuildRole.IsTeamRole(role)))
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        return Task.FromResult(PreconditionResult.FromError("You don't have permission to use this command."));
      }
}
