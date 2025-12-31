using System.Text.Json.Serialization;

namespace Saphira.Saphi.Core.Entity.Ranking;

public class SrPrRanking
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

    [JsonPropertyName("sr_pr")]
    public double SrPr { get; set; }

    [JsonPropertyName("sr_pr_by_category")]
    public Dictionary<string, double?>? SrPrByCategory { get; set; }

    [JsonPropertyName("player_count")]
    public int? PlayerCount { get; set; }
}
