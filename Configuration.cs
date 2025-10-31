namespace Saphira
{
    public class Configuration
    {
        public string BotToken { get; set; } = string.Empty;

        public ulong BotOwnerId { get; set; } = 0;

        public ulong GuildId { get; set; } = 0;
        
        public string MainChannel { get; set; } = string.Empty;

        public string SubmissionFeedChannel {  get; set; } = string.Empty;

        public List<string> CommandsAllowedChannels {  get; set; } = new List<string>();

        public string SaphiApiKey { get; set; } = string.Empty;

        public string SaphiApiBaseUrl { get; set; } = string.Empty;

        public int MaxLeaderboardEntries { get; set; } = 0;
    }
}
