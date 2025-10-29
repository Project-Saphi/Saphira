namespace Saphira.Discord.Guild
{
    public class GuildRoleManager
    {
        public List<string> GetToggleableRoles()
        {
            return new List<string>()
            {
                GuildRole.SaphiUpdatesRole,
                GuildRole.ServerUpdatesRole,
                GuildRole.WRFeedRole
            };
        }
    }
}
