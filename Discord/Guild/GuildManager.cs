using Discord;
using Discord.WebSocket;

namespace Saphira.Discord.Guild
{
    public class GuildManager
    {
        public List<IActivity> GetCTRStreamActivites(SocketGuild guild) =>
            guild.Users
                .SelectMany(u => u.Activities)
                .Where(a => a.Type == ActivityType.Streaming && Activity.IsCTRStream(a))
                .ToList();
    }
}
