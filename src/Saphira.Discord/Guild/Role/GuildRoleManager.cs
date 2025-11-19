namespace Saphira.Discord.Guild.Role;

public class GuildRoleManager
{
    public List<string> GetToggleableRoles()
    {
        return
        [
            GuildRole.SaphiUpdatesRole,
            GuildRole.ServerUpdatesRole,
            GuildRole.WRFeedRole
        ];
    }
}
