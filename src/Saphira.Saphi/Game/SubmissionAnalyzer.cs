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

    public async Task<CustomTrackStandard?> GetStandard(RecentSubmission submission)
    {
        var customTrack = await GetCustomTrackForSubmissionAsync(submission.TrackId.ToString());

        if (customTrack == null || customTrack.Standards == null)
        {
            return null;
        }

        CustomTrackStandard? submissionStandard = null;
        var standards = customTrack.Standards.Where(s => s.Type == submission.CategoryId.ToString()).OrderBy(s => s.Time).ToList();

        foreach (var standard in standards)
        {
            if (submission.Score < int.Parse(standard.Time))
            {
                submissionStandard = standard;
                break;
            }
        }

        return submissionStandard;
    }

    private async Task<CustomTrack?> GetCustomTrackForSubmissionAsync(string customTrackId)
    {
        var result = await client.GetCustomTracksAsync();

        if (!result.Success || result.Response == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Unable to fetch custom tracks: {result.ErrorMessage ?? "Unknown error"}");
            return null;
        }

        return result.Response.Data.FirstOrDefault(c => c.Id == customTrackId);
    }
}
