namespace Saphira.Util.EmoteMapper;

public class StreamingPlatformEmoteMapper
{
    private static readonly JsonResourceEmoteMapper<StreamingPlatformData> _mapper = new("streaming_platforms.json");

    public static string? MapStreamingPlatformToEmote(string streamingPlatform)
    {
        return _mapper.MapToEmote(streamingPlatform);
    }

    private class StreamingPlatformData : IEmoteData
    {
        public string Name { get; set; } = string.Empty;
        public string Emote { get; set; } = string.Empty;
    }
}
