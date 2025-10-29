namespace Saphira.Discord.Messaging
{
    public class MessageTextFormat
    {
        public static string Bold(string text)
        {
            return $"**{text}**";
        }

        public static string Italic(string text)
        {
            return $"*{text}*";
        }

        public static string Underline(string text)
        {
            return $"__{text}__";
        }

        public static string Strikethrough(string text)
        {
            return $"~~{text}~~";
        }

        public static string Code(string text)
        {
            return $"`{text}`";
        }

        public static string CodeBlock(string text, string language = "")
        {
            if (string.IsNullOrEmpty(language))
            {
                return $"```\n{text}\n```";
            }
            return $"```{language}\n{text}\n```";
        }

        public static string Quote(string text)
        {
            return $"> {text}";
        }

        public static string BlockQuote(string text)
        {
            return $">>> {text}";
        }

        public static string Spoiler(string text)
        {
            return $"||{text}||";
        }

        public static string Heading1(string text)
        {
            return $"# {text}";
        }

        public static string Heading2(string text)
        {
            return $"## {text}";
        }

        public static string Heading3(string text)
        {
            return $"### {text}";
        }

        public static string MaskedLink(string text, string url)
        {
            return $"[{text}]({url})";
        }

        public static string Mention(ulong userId)
        {
            return $"<@{userId}>";
        }

        public static string MentionChannel(ulong channelId)
        {
            return $"<#{channelId}>";
        }

        public static string MentionRole(ulong roleId)
        {
            return $"<@&{roleId}>";
        }

        public static string Timestamp(long unixTimestamp, string format = "f")
        {
            return $"<t:{unixTimestamp}:{format}>";
        }

        public static string BulletList(params string[] items)
        {
            return string.Join("\n", items.Select(item => $"- {item}"));
        }

        public static string NumberedList(params string[] items)
        {
            return string.Join("\n", items.Select((item, index) => $"{index + 1}. {item}"));
        }
    }
}
