using Discord;
using Discord.WebSocket;

namespace Saphira.Discord.Guild
{
    public class GuildManager
    {
        public List<IActivity> GetCTRStreamActivites(SocketGuild guild)
        {
            var livestreams = new List<IActivity>();

            foreach (var guildUser in guild.Users)
            {
                var ctrStreamingActivity = guildUser.Activities.FirstOrDefault(activity => activity.Type == ActivityType.Streaming && Activity.IsCTRStream(activity));
                
                if (ctrStreamingActivity != null)
                {
                    livestreams.Add(ctrStreamingActivity);
                }
            }

            return livestreams;
        }
    }
}
