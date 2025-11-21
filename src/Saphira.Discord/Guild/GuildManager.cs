using Discord;
using Discord.WebSocket;
using Saphira.Discord.Presence;

namespace Saphira.Discord.Guild;

public class GuildManager
{
    public static List<(SocketGuildUser User, IActivity Activity)> GetCTRStreamers(SocketGuild guild)
    {
        var streamers = new List<(SocketGuildUser, IActivity)>();

        foreach (var user in guild.Users)
        {
            var streamingActivity = user.Activities.FirstOrDefault(a => a.Type == ActivityType.Streaming);

            if (streamingActivity == null)
            {
                continue;
            }

            if (Activity.IsCTRStream(streamingActivity))
            {
                streamers.Add((user, streamingActivity));
                continue;
            }
        }

        return streamers;
    }
}
