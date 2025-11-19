using Discord;

namespace Saphira.Discord;

public class Activity
{
    public static bool IsCTRStream(IActivity activity)
    {
        return activity.Type == ActivityType.Streaming && activity.Name.Contains("crash team racing", StringComparison.CurrentCultureIgnoreCase);
    }
}
