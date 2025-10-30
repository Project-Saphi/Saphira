using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity
{
    public class Character
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("default_engine")]
        public string DefaultEngine { get; set; }
    }
}
