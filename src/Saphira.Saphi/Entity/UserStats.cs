using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class UserStats
{
    [JsonPropertyName("total_points")]
    public int TotalPoints { get; set; }

    [JsonPropertyName("course_points")]
    public int CoursePoints { get; set; }

    [JsonPropertyName("lap_points")]
    public int LapPoints { get; set; }

    [JsonPropertyName("tracks_submitted")]
    public int TracksSubmitted { get; set; }

    [JsonPropertyName("first_places")]
    public int FirstPlaces { get; set; }

    [JsonPropertyName("podium_finishes")]
    public int PodiumFinishes { get; set; }

    [JsonPropertyName("unique_tracks")]
    public int UniqueTracks { get; set; }

    [JsonPropertyName("course_tracks")]
    public int CourseTracks { get; set; }

    [JsonPropertyName("lap_tracks")]
    public int LapTracks { get; set; }

    [JsonPropertyName("first_submission")]
    public string? FirstSubmission { get; set; }

    [JsonPropertyName("last_submission")]
    public string? LastSubmission { get; set; }
}
