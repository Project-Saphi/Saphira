using Discord.WebSocket;

namespace Saphira.Discord.Messaging;

public class RestrictedContentDetector
{
    public bool MessageContainsRestrictedContent(SocketMessage message)
    {
        return message.Attachments.Count != 0 || message.Embeds.Count != 0 || MessageContainsUrl(message.Content);
    }

    public bool MessageContainsUrl(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        var urlPatterns = new[] { "http://", "https://", "www." };
        return urlPatterns.Any(pattern => content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}
