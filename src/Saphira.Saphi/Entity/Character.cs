using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class Character
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("default_engine_id")]
    public int DefaultEngineId { get; set; }

    [JsonPropertyName("default_engine_name")]
    public string DefaultEngineName { get; set; }
}
