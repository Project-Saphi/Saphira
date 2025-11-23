namespace Saphira.Core.Twitch;

public class Twitch
{
    public static string GetStreamThumbnailUrl(string accountName)
    {
        if (string.IsNullOrEmpty(accountName))
        {
            return string.Empty;
        }

        return $"https://static-cdn.jtvnw.net/previews-ttv/live_user_{accountName}-1920x1080.jpg";
    }

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
