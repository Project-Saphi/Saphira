using Discord.WebSocket;

namespace Saphira.Discord
{
    public class GuildMember
    {
        public static bool IsTeamMember(SocketUser user)
        {
            var socketGuildUser = user as SocketGuildUser;

            if (socketGuildUser == null)
            {
                return false;
            }

            return socketGuildUser.Roles.Any(role => GuildRole.IsTeamRole(role));
        }
    }
}
