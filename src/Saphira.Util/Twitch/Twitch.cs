namespace Saphira.Util.Twitch;

public class Twitch
{
    public static string? ExtractAccountNameFromStreamUrl(string streamUrl)
    {
        if (string.IsNullOrEmpty(streamUrl))
        {
            return null;
        }

        var uri = new Uri(streamUrl);
        var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        return pathSegments.Length > 0 ? pathSegments[0] : null;
    }
}
