using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class EngineType
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
