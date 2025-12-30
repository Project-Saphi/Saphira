using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity.Ranking;

public class PointsRanking
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

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("points_by_category")]
    public Dictionary<string, int?>? PointsByCategory { get; set; }

    [JsonPropertyName("first_places")]
    public int? FirstPlaces { get; set; }

    [JsonPropertyName("podium_finishes")]
    public int? PodiumFinishes { get; set; }

    [JsonPropertyName("player_count")]
    public int? PlayerCount { get; set; }
}
