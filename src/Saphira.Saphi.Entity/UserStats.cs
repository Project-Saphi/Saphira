using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class UserStats
{
    [JsonPropertyName("total_points")]
    public string TotalPoints { get; set; }

    [JsonPropertyName("course_points")]
    public string CoursePoints { get; set; }

    [JsonPropertyName("lap_points")]
    public string LapPoints { get; set; }

    [JsonPropertyName("tracks_submitted")]
    public string TracksSubmitted { get; set; }

    [JsonPropertyName("first_places")]
    public string FirstPlaces { get; set; }

    [JsonPropertyName("podium_finishes")]
    public string PodiumFinishes { get; set; }

    [JsonPropertyName("unique_tracks")]
    public string UniqueTracks { get; set; }

    [JsonPropertyName("course_tracks")]
    public string CourseTracks { get; set; }

    [JsonPropertyName("lap_tracks")]
    public string LapTracks { get; set; }

    [JsonPropertyName("first_submission")]
    public string? FirstSubmission { get; set; }

    [JsonPropertyName("last_submission")]
    public string? LastSubmission { get; set; }
}
