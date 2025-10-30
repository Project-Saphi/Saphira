using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity
{
    public class CustomTrack
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("crc32_hash")]
        public string Crc32Hash { get; set; }

        [JsonPropertyName("has_files")]
        public string HasFiles { get; set; }

        [JsonPropertyName("file_1_name")]
        public string? File1Name { get; set; }

        [JsonPropertyName("file_2_name")]
        public string? File2Name { get; set; }

        [JsonPropertyName("standards")]
        public List<CustomTrackStandard>? Standards { get; set; }
    }
}
