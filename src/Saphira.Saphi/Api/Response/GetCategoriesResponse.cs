using System.Text.Json.Serialization;
using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Api.Response;

public class GetCategoriesResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public List<Category> Data { get; set; }
}
