using Discord;
using Saphira.Core.Logging;

namespace Saphira.Core.Twitch;

public class TwitchClient(HttpClient httpClient, IMessageLogger logger)
{
    public async Task<Stream?> FetchStreamPreview(string accountName)
    {
        try
        {
            if (!string.IsNullOrEmpty(accountName))
            {
                var thumbnailUrl = $"https://static-cdn.jtvnw.net/previews-ttv/live_user_{accountName}-1920x1080.jpg";
                return await httpClient.GetStreamAsync(thumbnailUrl);
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch Twitch thumbnail for account {accountName}: {ex.Message}");
            return null;
        }
    }
}
