using System.Text.Json.Serialization;
using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Api.Response;

public class GetEngineTypesResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public List<EngineType> Data { get; set; }
}
