using System.Text.Json.Serialization;

namespace Saphira.Saphi.Api.Response;

public class SaphiApiResponse<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();

    [JsonPropertyName("meta")]
    public PaginationMeta Meta { get; set; } = new();
}

public class PaginationMeta
{
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }
}
