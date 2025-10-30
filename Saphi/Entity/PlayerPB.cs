using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity
{
    public class PlayerPB
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("track_id")]
        public string TrackId { get; set; }

        [JsonPropertyName("track_name")]
        public string TrackName { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("holder")]
        public string Holder { get; set; }

        [JsonPropertyName("country_id")]
        public string CountryId { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }
    }
}
