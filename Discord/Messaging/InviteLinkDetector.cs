using System.Text.RegularExpressions;

namespace Saphira.Discord.Messaging
{
    public class InviteLinkDetector
    {
        private readonly Regex InviteLinkPattern = new Regex(
            @"(https?://)?(www\.)?(discord\.(gg|io|me|li)|discordapp\.com/invite)/[a-zA-Z0-9]+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public bool MessageContainsInviteLink(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return InviteLinkPattern.IsMatch(text);
        }

        public IEnumerable<string> ExtractInviteLinks(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<string>();

            var matches = InviteLinkPattern.Matches(text);
            return matches.Select(m => m.Value);
        }

        public string RemoveInviteLinks(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return InviteLinkPattern.Replace(text, "[invite link removed]");
        }
    }
}
