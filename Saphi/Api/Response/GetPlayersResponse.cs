using Saphira.Saphi.Entity;
using System.Text.Json.Serialization;

namespace Saphira.Saphi.Api.Response;

public class GetPlayersResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public List<Player> Data { get; set; }
}
