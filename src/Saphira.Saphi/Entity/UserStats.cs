using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class UserStats
{
    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }

    [JsonPropertyName("total_time")]
    public int TotalTime { get; set; }

    [JsonPropertyName("total_time_formatted")]
    public string? TotalTimeFormatted { get; set; }

    [JsonPropertyName("avg_finish")]
    public double AvgFinish { get; set; }

    [JsonPropertyName("avg_standard")]
    public double AvgStandard { get; set; }

    [JsonPropertyName("avg_sr_pr")]
    public double AvgSrPr { get; set; }

    [JsonPropertyName("first_places")]
    public int FirstPlaces { get; set; }

    [JsonPropertyName("podium_finishes")]
    public int PodiumFinishes { get; set; }

    [JsonPropertyName("tracks_submitted")]
    public int TracksSubmitted { get; set; }

    [JsonPropertyName("first_submission")]
    public string? FirstSubmission { get; set; }

    [JsonPropertyName("last_submission")]
    public string? LastSubmission { get; set; }
}
