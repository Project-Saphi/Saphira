using System.Text.RegularExpressions;

namespace Saphira.Discord.Messaging;

public class InviteLinkDetector
{
    private readonly Regex InviteLinkPattern = new(
        @"(https?://)?(www\.)?(discord\.(gg|io|me|li)|discordapp\.com/invite)/[a-zA-Z0-9]+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(500)
    );

    public bool MessageContainsInviteLink(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return InviteLinkPattern.IsMatch(text);
    }
}
