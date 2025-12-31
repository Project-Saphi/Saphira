using System.Text.Json.Serialization;

namespace Saphira.Saphi.Core.Entity.Ranking;

public class AverageRankRanking
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

    [JsonPropertyName("average_rank")]
    public double AverageRank { get; set; }

    [JsonPropertyName("average_by_category")]
    public Dictionary<string, double?>? AverageByCategory { get; set; }

    [JsonPropertyName("standard_id")]
    public int? StandardId { get; set; }

    [JsonPropertyName("standard_name")]
    public string? StandardName { get; set; }

    [JsonPropertyName("standard_by_category")]
    public Dictionary<string, int?>? StandardByCategory { get; set; }

    [JsonPropertyName("standard_name_by_category")]
    public Dictionary<string, string>? StandardNameByCategory { get; set; }

    [JsonPropertyName("player_count")]
    public int? PlayerCount { get; set; }
}
