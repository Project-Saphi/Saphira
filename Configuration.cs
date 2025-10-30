namespace Saphira
{
    public class Configuration
    {
        public string BotToken { get; set; } = string.Empty;

        public ulong BotOwnerId { get; set; } = 0;

        public ulong GuildId { get; set; } = 0;
        
        public string MainChannelName { get; set; } = string.Empty;

        public string SaphiApiKey { get; set; } = string.Empty;

        public string SaphiApiBaseUrl { get; set; } = string.Empty;

        public int MaxLeaderboardEntries { get; set; } = 0;
    }
}
