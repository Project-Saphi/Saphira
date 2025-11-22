using Discord;

namespace Saphira.Discord.Presence;

public class Activity
{
    public enum StreamingPlatform
    {
        Twitch = 7689143,
        Youtube = 10893626
    }

    private static readonly string[] CtrAliases =
    [
        "crash team racing",
        "ctr",
        "crash racing",
        "crash bandicoot racing",
        "cbr",
        "project saphi"
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

    public static StreamingPlatform? GetStreamingPlatform(string activityName)
    {
        var streamingPlatforms = new Dictionary<string, StreamingPlatform>()
        {
            { "Twitch", StreamingPlatform.Twitch },
            { "YouTube", StreamingPlatform.Youtube }
        };

        if (!streamingPlatforms.TryGetValue(activityName, out var streamingPlatform))
        {
            return null;
        }

        return streamingPlatform;
    }
}
