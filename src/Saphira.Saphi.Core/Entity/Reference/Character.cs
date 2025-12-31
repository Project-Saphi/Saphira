using System.Text.Json.Serialization;

namespace Saphira.Saphi.Core.Entity.Reference;

public class Character
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("game_id")]
    public int GameId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("default_engine_id")]
    public int DefaultEngineId { get; set; }

    [JsonPropertyName("engine_name")]
    public string EngineName { get; set; }

    [JsonPropertyName("submission_count")]
    public int SubmissionCount { get; set; }
}
