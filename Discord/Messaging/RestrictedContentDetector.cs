using Discord.WebSocket;

namespace Saphira.Discord.Messaging
{
    public class RestrictedContentDetector
    {
        public bool MessageContainsRestrictedContent(SocketMessage message)
        {
            return (message.Attachments.Any() || message.Embeds.Any() || MessageContainsUrl(message.Content));
        }

        public bool MessageContainsUrl(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return false;
            }

            return content.Contains("http://", StringComparison.OrdinalIgnoreCase) ||
                   content.Contains("https://", StringComparison.OrdinalIgnoreCase) ||
                   content.Contains("www.", StringComparison.OrdinalIgnoreCase);
        }
    }
}
