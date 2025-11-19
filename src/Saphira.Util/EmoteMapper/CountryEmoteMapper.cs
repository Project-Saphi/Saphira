namespace Saphira.Util.EmoteMapper;

public class CountryEmoteMapper
{
    private static readonly JsonResourceEmoteMapper<CountryData> _mapper = new("countries.json", defaultEmote: ":united_nations:");

    public static string? MapCountryToEmote(string countryName)
    {
        return _mapper.MapToEmote(countryName);
    }

    private class CountryData : IEmoteData
    {
        public string Name { get; set; } = string.Empty;
        public string Emote { get; set; } = string.Empty;
    }
}
