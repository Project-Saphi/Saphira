namespace Saphira.Util.Mapper;

public class CharacterEmoteMapper
{
    private static readonly JsonResourceEmoteMapper<CharacterData> _mapper = new("characters.json");

    public static string? MapCharacterToEmote(string characterName)
    {
        return _mapper.MapToEmote(characterName);
    }

    private class CharacterData : IEmoteData
    {
        public string Name { get; set; } = string.Empty;
        public string Emote { get; set; } = string.Empty;
    }
}
