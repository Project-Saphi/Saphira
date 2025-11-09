using Discord.Interactions;
using Saphira.Commands.Metadata;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class PingCommand : BaseCommand
{
    public override CommandMetadata GetMetadata()
    {
        return new CommandMetadata(
            "Check the bot's latency",
            "/ping"
        );
    }

    [SlashCommand("ping", "Check the bot's latency")]
    public async Task HandleCommand()
    {
        var latency = Context.Client.Latency;

        var uptime = DateTime.UtcNow - Program.StartTime;
        var uptimeString = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";

        var ping = new[]
        {
            $"{MessageTextFormat.Bold("Latency")}: {latency}ms",
            $"{MessageTextFormat.Bold("Uptime")}: {uptimeString}"
        };

        var successAlert = new SuccessAlertEmbedBuilder(String.Join("\n", ping));
        await RespondAsync(embed: successAlert.Build());
    }
}
