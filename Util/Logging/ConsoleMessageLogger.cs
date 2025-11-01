using Discord;

namespace Saphira.Util.Logging;

public class ConsoleMessageLogger : IMessageLogger
{
    public void Log(LogSeverity severity, string source, string message, Exception? exception = null)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = GetLogColor(severity);

        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine($"[{timestamp}] [{source}] {message}");

        if (exception != null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.ToString());
        }

        Console.ForegroundColor = originalColor;
    }

    public void Log(LogMessage message)
    {
        Log(message.Severity, message.Source, message.Message, message.Exception);
    }

    private static ConsoleColor GetLogColor(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => ConsoleColor.DarkRed,
            LogSeverity.Error => ConsoleColor.Red,
            LogSeverity.Warning => ConsoleColor.Yellow,
            LogSeverity.Info => ConsoleColor.White,
            LogSeverity.Verbose => ConsoleColor.Gray,
            LogSeverity.Debug => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };
    }
}
