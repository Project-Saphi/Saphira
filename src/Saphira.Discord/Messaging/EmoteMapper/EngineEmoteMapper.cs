namespace Saphira.Discord.Messaging.EmoteMapper;

public class EngineEmoteMapper
{
    private static readonly JsonResourceEmoteMapper<EngineData> _mapper = new("engines.json");

    public static string? MapEngineToEmote(string engineName)
    {
        return _mapper.MapToEmote(engineName);
    }
}

public class EngineData : IEmoteData
{
    public string Name { get; set; } = string.Empty;
    public string Emote { get; set; } = string.Empty;
}
