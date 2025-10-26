using Discord;

namespace Saphira.Discord
{
    public class GuildRole
    {
        public static string TeamRole = "Saphi Team";

        public static bool IsTeamRole(IRole role)
        {
            return role.Name == GuildRole.TeamRole;
        }
    }
}
