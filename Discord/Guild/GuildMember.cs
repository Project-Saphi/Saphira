using Discord.WebSocket;

namespace Saphira.Discord.Guild
{
    public class GuildMember
    {
        public static bool IsTeamMember(SocketUser user)
            => (user as SocketGuildUser)?.Roles.Any(GuildRole.IsTeamRole) ?? false;
    }
}
