namespace Saphira.Discord.Messaging.EmoteMapper;

public class TierEmoteMapper
{
	private static readonly JsonResourceEmoteMapper<TierData> _mapper = new("tiers.json");

	public static string? MapTierToEmote(string tierId)
	{
		return _mapper.MapToEmote(tierId);
	}

	private class TierData : IEmoteData
	{
		public string Name { get; set; } = string.Empty;
		public string Emote { get; set; } = string.Empty;
	}
}
