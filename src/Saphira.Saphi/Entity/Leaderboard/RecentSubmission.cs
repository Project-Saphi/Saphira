using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity.Leaderboard;

public class RecentSubmission
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("country_id")]
    public int? CountryId { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("track_id")]
    public int TrackId { get; set; }

    [JsonPropertyName("track_name")]
    public string TrackName { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("category_name")]
    public string CategoryName { get; set; }

    [JsonPropertyName("character_id")]
    public int CharacterId { get; set; }

    [JsonPropertyName("character_name")]
    public string CharacterName { get; set; }

    [JsonPropertyName("engine_id")]
    public int EngineId { get; set; }

    [JsonPropertyName("engine_name")]
    public string EngineName { get; set; }

    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("time_formatted")]
    public string TimeFormatted { get; set; }

    [JsonPropertyName("submitted_at")]
    public string SubmittedAt { get; set; }

    [JsonPropertyName("is_world_record")]
    public bool IsWorldRecord { get; set; }
}
