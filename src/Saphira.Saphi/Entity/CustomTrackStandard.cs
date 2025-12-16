using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class CustomTrackStandard
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("track_id")]
    public int TrackId { get; set; }

    [JsonPropertyName("tier_id")]
    public int TierId { get; set; }

    [JsonPropertyName("tier_name")]
    public string? TierName { get; set; }

    [JsonPropertyName("tier_numeric_value")]
    public int TierNumericValue { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("time_formatted")]
    public string? TimeFormatted { get; set; }
}
