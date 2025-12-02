using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class CustomTrackStandard
{
    [JsonPropertyName("tier_id")]
    public int TierId { get; set; }

    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }
}
