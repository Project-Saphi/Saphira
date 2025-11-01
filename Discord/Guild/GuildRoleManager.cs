namespace Saphira.Discord.Guild;

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
