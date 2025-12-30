using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity.User;

public class UserStats
{
    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }

    [JsonPropertyName("course_points")]
    public int CoursePoints { get; set; }

    [JsonPropertyName("lap_points")]
    public int LapPoints { get; set; }

    [JsonPropertyName("total_time")]
    public int TotalTime { get; set; }

    [JsonPropertyName("total_time_formatted")]
    public string? TotalTimeFormatted { get; set; }

    [JsonPropertyName("average_finish")]
    public double AverageFinish { get; set; }

    [JsonPropertyName("average_standard")]
    public double AverageStandard { get; set; }

    [JsonPropertyName("average_sr_pr")]
    public double AverageSrPr { get; set; }

    [JsonPropertyName("first_places")]
    public int FirstPlaces { get; set; }

    [JsonPropertyName("podium_finishes")]
    public int PodiumFinishes { get; set; }

    [JsonPropertyName("tracks_submitted")]
    public int TracksSubmitted { get; set; }

    [JsonPropertyName("completed_tracks")]
    public int CompletedTracks { get; set; }

    [JsonPropertyName("total_tracks")]
    public int TotalTracks { get; set; }

    [JsonPropertyName("first_submission_at")]
    public string? FirstSubmissionAt { get; set; }

    [JsonPropertyName("last_submission_at")]
    public string? LastSubmissionAt { get; set; }

    [JsonPropertyName("most_played_track")]
    public MostPlayedItem? MostPlayedTrack { get; set; }

    [JsonPropertyName("most_played_character")]
    public MostPlayedItem? MostPlayedCharacter { get; set; }

    [JsonPropertyName("most_played_engine")]
    public MostPlayedItem? MostPlayedEngine { get; set; }
}

public class MostPlayedItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("submission_count")]
    public int SubmissionCount { get; set; }
}
