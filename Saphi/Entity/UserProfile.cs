using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class UserProfile
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("country")]
    public UserCountry Country { get; set; }

    [JsonPropertyName("stats")]
    public UserStats Stats { get; set; }
}
