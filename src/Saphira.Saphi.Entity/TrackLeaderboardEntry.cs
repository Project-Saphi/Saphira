using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class TrackLeaderboardEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; }

    [JsonPropertyName("category_name")]
    public string CategoryName { get; set; }

    [JsonPropertyName("character_id")]
    public string CharacterId { get; set; }

    [JsonPropertyName("character_name")]
    public string CharacterName { get; set; }

    [JsonPropertyName("engine_id")]
    public string EngineId { get; set; }

    [JsonPropertyName("engine_name")]
    public string EngineName { get; set; }

    [JsonPropertyName("country_id")]
    public string CountryId { get; set; }

    [JsonPropertyName("country_name")]
    public string CountryName { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("MinScore")]
    public string MinScore { get; set; }

    [JsonPropertyName("holder")]
    public string Holder { get; set; }
}
