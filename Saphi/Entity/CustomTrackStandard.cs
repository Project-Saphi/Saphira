using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity
{
    public class CustomTrackStandard
    {
        [JsonPropertyName("tier_id")]
        public string TierId { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
