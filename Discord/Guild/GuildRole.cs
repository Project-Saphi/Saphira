using Discord;

namespace Saphira.Discord.Guild;

public class GuildRole
{
    public static readonly string TeamRole = "Saphi Team";
    public static readonly string SaphiUpdatesRole = "Saphi Updates";
    public static readonly string ServerUpdatesRole = "Server Updates";
    public static readonly string VerifiedRole = "Verified";
    public static readonly string WRFeedRole = "WR Feed";

    public static bool IsTeamRole(IRole role) => role.Name == TeamRole;

    public static bool IsSaphiUpdatesRole(IRole role) => role.Name == SaphiUpdatesRole;

    public static bool IsServerUpdatesRole(IRole role) => role.Name == ServerUpdatesRole;

    public static bool IsVerifiedRole(IRole role) => role.Name == VerifiedRole;

    public static bool IsWRFeedRole(IRole role) => role.Name == WRFeedRole;
}
