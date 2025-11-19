using Discord;

namespace Saphira.Discord.Logging;

public interface IMessageLogger
{
    public void Log(LogMessage message);

    public void Log(LogSeverity severity, string source, string message, Exception? exception = null);
}
