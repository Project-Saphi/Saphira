using System.Text.Json.Serialization;
using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Api.Response;

public class GetCustomTracksResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public List<CustomTrack> Data { get; set; }
}
