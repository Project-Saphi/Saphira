using Discord;

namespace Saphira.Discord.Guild
{
    public class GuildRole
    {
        public static string TeamRole = "Saphi Team";
        public static string SaphiUpdatesRole = "Saphi Updates";
        public static string ServerUpdatesRole = "Server Updates";
        public static string VerifiedRole = "Verified";
        public static string WRFeedRole = "WR Feed";

        public static bool IsTeamRole(IRole role) => role.Name == TeamRole;

        public static bool IsSaphiUpdatesRole(IRole role) => role.Name == SaphiUpdatesRole;

        public static bool IsServerUpdatesRole(IRole role) => role.Name == ServerUpdatesRole;

        public static bool IsVerifiedRole(IRole role) => role.Name == VerifiedRole;

        public static bool IsWRFeedRole(IRole role) => role.Name == WRFeedRole;
    }
}
