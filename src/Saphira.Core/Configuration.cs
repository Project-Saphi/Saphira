namespace Saphira.Core;

public class Configuration
{
    public string BotToken { get; set; } = "";

    public ulong BotOwnerId { get; set; }

    public int MinimumLogLevel { get; set; }

    public ulong GuildId { get; set; }

    public string MainChannel { get; set; } = "";

    public string DmLogChannel { get; set; } = "";

    public string SubmissionFeedChannel { get; set; } = "";

    public string LivestreamsChannel { get; set; } = "";

    public List<string> CommandsAllowedChannels { get; set; } = new();

    public int MaxAutocompleteSuggestions { get; set; }

    public string SaphiApiKey { get; set; } = "";

    public string SaphiApiBaseUrl { get; set; } = "";
}
