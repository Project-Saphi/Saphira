using Discord.Interactions;
using Saphira.Core;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class PingCommand(Application application) : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata("/ping");
    }

    [SlashCommand("ping", "Check the bot's latency")]
    public async Task HandleCommand()
    {
        await DeferAsync();

        var latency = Context.Client.Latency;

        var uptime = DateTime.UtcNow - application.StartTime;
        var uptimeString = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";

        var ping = new[]
        {
            $"{MessageTextFormat.Bold("Latency")}: {latency}ms",
            $"{MessageTextFormat.Bold("Uptime")}: {uptimeString}"
        };

        var successAlert = new SuccessAlertEmbedBuilder(string.Join("\n", ping));
        await FollowupAsync(embed: successAlert.Build());
    }
}
