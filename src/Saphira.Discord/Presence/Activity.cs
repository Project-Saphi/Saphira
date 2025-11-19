using Discord;

namespace Saphira.Discord.Presence;

public class Activity
{
    private static readonly string[] CtrAliases =
    [
        "crash team racing",
        "ctr",
        "crash racing",
        "crash bandicoot racing",
        "cbr"
    ];

    public static bool IsCTRStream(IActivity activity)
    {
        if (activity.Type != ActivityType.Streaming)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(activity.Name) &&
            CtrAliases.Any(alias => activity.Name.Contains(alias, StringComparison.CurrentCultureIgnoreCase)))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(activity.Details) &&
            CtrAliases.Any(alias => activity.Details.Contains(alias, StringComparison.CurrentCultureIgnoreCase)))
        {
            return true;
        }

        return false;
    }

    public static bool IsCTRGame(IActivity activity)
    {
        if (activity.Type != ActivityType.Playing)
        {
            return false;
        }

        return !string.IsNullOrEmpty(activity.Name) && CtrAliases.Any(alias => activity.Name.Contains(alias, StringComparison.CurrentCultureIgnoreCase));
    }
}
