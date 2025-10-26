using Discord;

namespace Saphira.Discord
{
    public class AlertEmbedBuilder : EmbedBuilder
    {
        public enum AlertType {
            Primary,
            Info,
            Success,
            Warning,
            Error
        }

        public enum AlertColor
        {
            Primary = 7433898,
            Info = 3901635,
            Success = 7844437,
            Warning = 16763981,
            Error = 12458289
        }

        public enum AlertEmote
        {
            Primary,
            Info,
            Success,
            Warning,
            Error
        }

        public enum AlertTitle
        {
            Primary,
            Info,
            Success,
            Warning,
            Error
        }

        public AlertEmbedBuilder(string message)
        {
            SetAlertProperties(AlertColor.Primary, AlertEmote.Primary, AlertTitle.Primary, message);
        }

        protected virtual void SetAlertProperties(AlertColor color, AlertEmote emote, AlertTitle title, string message)
        {
            Fields.Clear();

            WithColor(new Color((uint)color));

            var fieldBuilder = new EmbedFieldBuilder();
            fieldBuilder.WithName($"{GetEmoteString(emote)} {GetTitleString(title)}");
            fieldBuilder.WithValue(message);

            AddField(fieldBuilder);
        }

        protected string GetEmoteString(AlertEmote emote)
        {
            var emotes = new List<string>();
            emotes.Add("📝");
            emotes.Add("ℹ️");
            emotes.Add("✅");
            emotes.Add("⚠️");
            emotes.Add("⛔");

            return emotes[(int) emote];
        }

        protected string GetTitleString(AlertTitle title)
        {
            var titles = new List<string>();
            titles.Add("Message");
            titles.Add("Info!");
            titles.Add("Success!");
            titles.Add("Warning!");
            titles.Add("Error!");

            return titles[(int) title];
        }
    }

    public class InfoAlertEmbedBuilder : AlertEmbedBuilder
    {
        public InfoAlertEmbedBuilder(string message) : base(message)
        {
            SetAlertProperties(AlertColor.Info, AlertEmote.Info, AlertTitle.Info, message);
        }
    }

    public class SuccessAlertEmbedBuilder : AlertEmbedBuilder
    {
        public SuccessAlertEmbedBuilder(string message) : base(message)
        {
            SetAlertProperties(AlertColor.Success, AlertEmote.Success, AlertTitle.Success, message);
        }
    }

    public class WarningAlertEmbedBuilder : AlertEmbedBuilder
    {
        public WarningAlertEmbedBuilder(string message) : base(message)
        {
            SetAlertProperties(AlertColor.Warning, AlertEmote.Warning, AlertTitle.Warning, message);
        }
    }

    public class ErrorAlertEmbedBuilder : AlertEmbedBuilder
    {
        public ErrorAlertEmbedBuilder(string message) : base(message)
        {
            SetAlertProperties(AlertColor.Error, AlertEmote.Error, AlertTitle.Error, message);
        }
    }
}
