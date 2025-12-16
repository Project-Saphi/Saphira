using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class UserProfile
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("country")]
    public UserCountry? Country { get; set; }

    [JsonPropertyName("stats")]
    public UserStats Stats { get; set; }
}
