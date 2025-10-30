﻿using Discord.Commands;
using Discord.WebSocket;
using Saphira.Discord.Guild;

namespace Saphira.Commands.Precondition
{
    public class RequireTeamMemberRole : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, 
            CommandInfo command, 
            IServiceProvider services
            )
        {
            var socketGuildUser = context.User as SocketGuildUser;

            if (socketGuildUser == null)
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
}
