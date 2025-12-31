using System.Text.Json.Serialization;

namespace Saphira.Saphi.Core.Entity.Ranking;

public class TotalTimeRanking
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("country_id")]
    public int? CountryId { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("rank")]
    public int Placement { get; set; }

    [JsonPropertyName("tracks_submitted")]
    public int TracksSubmitted { get; set; }

    [JsonPropertyName("total_tracks")]
    public int TotalTracks { get; set; }

    [JsonPropertyName("total_time")]
    public int TotalTime { get; set; }

    [JsonPropertyName("total_time_formatted")]
    public string? TotalTimeFormatted { get; set; }

    [JsonPropertyName("time_by_category")]
    public Dictionary<string, TimeCategoryInfo>? TimeByCategory { get; set; }

    [JsonPropertyName("player_count")]
    public int? PlayerCount { get; set; }
}

public class TimeCategoryInfo
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("total_formatted")]
    public string? TotalFormatted { get; set; }

    [JsonPropertyName("tracks")]
    public int Tracks { get; set; }

    [JsonPropertyName("submitted")]
    public int Submitted { get; set; }
}
