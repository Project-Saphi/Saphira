namespace Saphira;

public class Configuration
{
    public string BotToken { get; set; } = "";

    public ulong BotOwnerId { get; set; }

    public ulong GuildId { get; set; }

    public string MainChannel { get; set; } = "";

    public string SubmissionFeedChannel { get; set; } = "";

    public List<string> CommandsAllowedChannels { get; set; } = new();

    public string SaphiApiKey { get; set; } = "";

    public string SaphiApiBaseUrl { get; set; } = "";

    public int MaxLeaderboardEntries { get; set; }
}
