using Discord;
using Saphira.Core.Logging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Game;

public class SubmissionAnalyzer(ISaphiApiClient client, IMessageLogger logger)
{
    public async Task<bool> CheckIsWorldRecordAsync(RecentSubmission submission)
    {
        var worldRecordSubmission = await GetWorldRecordSubmissionAsync(submission.TrackId.ToString(), submission.CategoryId.ToString());

        if (worldRecordSubmission == null)
        {
            return false;
        }

        return worldRecordSubmission.Id == submission.Id.ToString(); 
    }

    private async Task<TrackLeaderboardEntry?> GetWorldRecordSubmissionAsync(string customTrackId, string categoryId)
    {
        var result = await client.GetTrackLeaderboardAsync(customTrackId, categoryId, forceRefresh: true);

        if (!result.Success || result.Response == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Unable to fetch the leaderboard for custom track {customTrackId}: {result.ErrorMessage ?? "Unable error"}");
            return null;
        }

        return result.Response.Data.FirstOrDefault(e => e.Rank == 1);
    }
}
