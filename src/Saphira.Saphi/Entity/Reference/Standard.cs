using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity.Reference;

public class Standard
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("display_order")]
    public int DisplayOrder { get; set; }

    [JsonPropertyName("numeric_value")]
    public int? NumericValue { get; set; }

    [JsonPropertyName("include_in_average")]
    public bool IncludeInAverage { get; set; }
}
