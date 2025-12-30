using System.Text.Json.Serialization;

namespace Saphira.Saphi.Api.Response;

public class RankingMeta : PaginationMeta
{
    [JsonPropertyName("ranking_type")]
    public string? RankingType { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("track_counts")]
    public TrackCounts? TrackCounts { get; set; }
}

public class TrackCounts
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("by_category")]
    public Dictionary<string, int>? ByCategory { get; set; }
}
