using Discord;
using Discord.Interactions;
using Saphira.Discord;

namespace Saphira.Commands
{
    public class General : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Check the bot's latency")]
        public async Task PingCommand()
        {
            var latency = Context.Client.Latency;

            var uptime = DateTime.UtcNow - Program.StartTime;
            var uptimeString = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";

            var ping = new List<string>();
            ping.Add($"**Latency**: {latency}ms");
            ping.Add($"**Uptime**: {uptimeString}");

            var successAlert = new SuccessAlertEmbedBuilder(String.Join("\n", ping));
            await RespondAsync(embed: successAlert.Build());
        }

        [SlashCommand("server", "Get information about this server")]
        public async Task ServerInfoCommand()
        {
            var guild = Context.Guild;

            if (guild == null)
            {
                await RespondAsync("This command can only be used in a server!", ephemeral: true);
                return;
            }

            var generalInfo = new List<string>();
            generalInfo.Add($"**Name**: {guild.Name}");
            generalInfo.Add($"**Description**: {guild.Description}");
            generalInfo.Add($"**Created**: {guild.CreatedAt:yyyy-MM-dd}");
            generalInfo.Add($"**Owner**: <@{guild.OwnerId}>");

            var statistics = new List<string>();
            statistics.Add($"**Members**: {guild.MemberCount}");
            statistics.Add($"**Emotes**: {guild.Emotes.Count}");
            statistics.Add($"**Roles**: {guild.Roles.Count}");
            statistics.Add($"**Channels**: {guild.Channels.Count}");
            
            var data = new List<string>();
            data.Add($"**Max Bitrate**: {guild.MaxBitrate}");
            data.Add($"**Max Upload Size**: {guild.MaxUploadLimit}");
            data.Add($"**Max Members**: {guild.MaxMembers}");

            var embed = new EmbedBuilder();

            var generalInfoField = new EmbedFieldBuilder();
            generalInfoField.WithName("General Information");
            generalInfoField.WithValue(String.Join("\n", generalInfo));
            generalInfoField.WithIsInline(true);

            var statisticsField = new EmbedFieldBuilder();
            statisticsField.WithName("Server Statistics");
            statisticsField.WithValue(String.Join("\n", statistics));
            statisticsField.WithIsInline(true);

            var dataField = new EmbedFieldBuilder();
            dataField.WithName("Data");
            dataField.WithValue(String.Join("\n", data));
            dataField.WithIsInline(true);

            embed.AddField(generalInfoField);
            embed.AddField(statisticsField);
            embed.AddField(dataField);

            await RespondAsync(embed: embed.Build());
        }
    }
}
