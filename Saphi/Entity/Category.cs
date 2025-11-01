using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class Category
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
