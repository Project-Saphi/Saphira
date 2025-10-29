namespace Saphira
{
    public class Configuration
    {
        public string BotToken { get; set; } = string.Empty;

        public ulong GuildId { get; set; } = 0;

        public ulong BotOwnerId { get; set; } = 0;

        public string SaphiApiKey { get; set; } = string.Empty;

        public string SaphiApiBaseUrl { get; set; } = string.Empty;
        public string MainChannelName {  get; set; } = string.Empty;
    }
}
