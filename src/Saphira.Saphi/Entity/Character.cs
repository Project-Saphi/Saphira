using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class Character
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("default_engine")]
    public int DefaultEngine { get; set; }
}
