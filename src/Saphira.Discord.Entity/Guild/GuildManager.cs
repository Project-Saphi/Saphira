using Discord;
using Discord.WebSocket;
using Saphira.Discord.Entity.Status;

namespace Saphira.Discord.Entity.Guild;

public class GuildManager
{
    public static List<IActivity> GetCTRStreamActivites(SocketGuild guild)
    {
        return [.. guild.Users
            .SelectMany(u => u.Activities)
            .Where(a => a.Type == ActivityType.Streaming && Activity.IsCTRStream(a))];
    }

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

            var hasCtrgame = user.Activities.Any(a => Activity.IsCTRGame(a));
            if (hasCtrgame)
            {
                streamers.Add((user, streamingActivity));
            }
        }

        return streamers;
    }
}
