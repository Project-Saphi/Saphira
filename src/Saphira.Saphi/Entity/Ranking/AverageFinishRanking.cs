using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity.Ranking;

public class AverageFinishRanking
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

    [JsonPropertyName("average_finish")]
    public double Average { get; set; }

    [JsonPropertyName("average_by_category")]
    public Dictionary<string, double?>? AverageByCategory { get; set; }

    [JsonPropertyName("player_count")]
    public int? PlayerCount { get; set; }
}
