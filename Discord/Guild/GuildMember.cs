using Discord.WebSocket;

namespace Saphira.Discord.Guild
{
    public class GuildMember
    {
        public static bool IsTeamMember(SocketUser user) => (user as SocketGuildUser)?.Roles.Any(GuildRole.IsTeamRole) ?? false;

        public static bool IsVerified(SocketUser user) => (user as SocketGuildUser)?.Roles.Any(GuildRole.IsVerifiedRole) ?? false;

        public static bool IsNewUser(SocketUser user)
        {
            var guildUser = user as SocketGuildUser;

            if (guildUser == null || guildUser.JoinedAt == null)
            {
                return false;
            }

            var timeSinceJoined = DateTimeOffset.UtcNow - guildUser.JoinedAt.Value;
            return timeSinceJoined.TotalHours < 12;
        }
    }
}
