using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class Country
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
