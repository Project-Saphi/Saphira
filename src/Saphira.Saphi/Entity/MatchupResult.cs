using System.Text.Json.Serialization;

namespace Saphira.Saphi.Entity;

public class MatchupResult
{
    [JsonPropertyName("player1")]
    public MatchupPlayer Player1 { get; set; }

    [JsonPropertyName("player2")]
    public MatchupPlayer Player2 { get; set; }

    [JsonPropertyName("ties")]
    public int Ties { get; set; }

    [JsonPropertyName("total_tracks")]
    public int TotalTracks { get; set; }

    [JsonPropertyName("overall_winner")]
    public int? OverallWinner { get; set; }

    [JsonPropertyName("comparisons")]
    public List<MatchupComparison> Comparisons { get; set; } = new();
}

public class MatchupPlayer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("country_id")]
    public int? CountryId { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("wins")]
    public int Wins { get; set; }
}

public class MatchupComparison
{
    [JsonPropertyName("track_id")]
    public int TrackId { get; set; }

    [JsonPropertyName("track_name")]
    public string TrackName { get; set; }

    [JsonPropertyName("track_author")]
    public string? TrackAuthor { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("category_name")]
    public string CategoryName { get; set; }

    [JsonPropertyName("player1_time")]
    public int Player1Time { get; set; }

    [JsonPropertyName("player2_time")]
    public int Player2Time { get; set; }

    [JsonPropertyName("player1_time_formatted")]
    public string Player1TimeFormatted { get; set; }

    [JsonPropertyName("player2_time_formatted")]
    public string Player2TimeFormatted { get; set; }

    [JsonPropertyName("player1_character_id")]
    public int Player1CharacterId { get; set; }

    [JsonPropertyName("player1_character_name")]
    public string? Player1CharacterName { get; set; }

    [JsonPropertyName("player2_character_id")]
    public int Player2CharacterId { get; set; }

    [JsonPropertyName("player2_character_name")]
    public string? Player2CharacterName { get; set; }

    [JsonPropertyName("difference")]
    public int Difference { get; set; }

    [JsonPropertyName("difference_formatted")]
    public string DifferenceFormatted { get; set; }

    [JsonPropertyName("winner")]
    public int Winner { get; set; }
}
