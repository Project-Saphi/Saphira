using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class TrackLeaderboardEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; }

    [JsonPropertyName("character_id")]
    public string CharacterId { get; set; }

    [JsonPropertyName("country_id")]
    public string CountryId { get; set; }

    [JsonPropertyName("MinScore")]
    public string MinScore { get; set; }

    [JsonPropertyName("holder")]
    public string Holder { get; set; }
}
