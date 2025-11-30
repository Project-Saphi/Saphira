using Discord;
using Saphira.Core.Logging;
using System.Diagnostics;

namespace Saphira.Core.Twitch;

public class TwitchClient(HttpClient httpClient, IMessageLogger logger)
{
    public async Task<Stream?> FetchStreamPreview(string accountName)
    {
        var stopWatch = new Stopwatch();

        try
        {
            if (!string.IsNullOrEmpty(accountName))
            {
                stopWatch.Start();
                logger.Log(LogSeverity.Debug, "Saphira", $"Fetching stream preview for twitch.tv/{accountName} ...");

                var thumbnailUrl = Twitch.GetStreamThumbnailUrl(accountName);
                var stream = await httpClient.GetStreamAsync(thumbnailUrl);

                stopWatch.Stop();
                logger.Log(LogSeverity.Debug, "Saphira", $"Fetched stream preview for twitch.tv/{accountName} in {stopWatch.ElapsedMilliseconds}ms");

                return stream;
            }

            logger.Log(LogSeverity.Warning, "Saphira", "Cannot fetch stream preview for empty account name");
            return null;
        }
        catch (Exception ex)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch stream preview for twitch.tv/{accountName}: {ex.Message}");
            return null;
        }
    }
}
