using System.Text.Json.Serialization;
using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Api.Response;

public class GetCustomTrackResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public CustomTrack Data { get; set; }
}
