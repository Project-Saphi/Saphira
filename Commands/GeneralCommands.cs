using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class GeneralCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly GuildManager _guildManager;

        public GeneralCommands(GuildManager guildManager)
        {
            _guildManager = guildManager;
        }

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [SlashCommand("livestreams", "List all CTR livestreams from server members")]
        public async Task LivestreamsCommand()
        {
            var livestreams = _guildManager.GetCTRStreamActivites(Context.Guild);

            if (livestreams.Count > 0)
            {
                await RespondAsync("People are streaming CTR ...");
                return;
            }
            else
            {
                var warningAlert = new WarningAlertEmbedBuilder("There is currently nobody streaming CTR.");
                await RespondAsync(embed: warningAlert.Build());
                return;
            }
        }

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [SlashCommand("ping", "Check the bot's latency")]
        public async Task PingCommand()
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

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [SlashCommand("server", "Get information about this server")]
        public async Task ServerInfoCommand()
        {
            var guild = Context.Guild;

            if (guild == null)
            {
                await RespondAsync("This command can only be used in a server!", ephemeral: true);
                return;
            }

            var generalInfo = new[]
            {
                $"{MessageTextFormat.Bold("Name")}: {guild.Name}",
                $"{MessageTextFormat.Bold("Description")}: {guild.Description}",
                $"{MessageTextFormat.Bold("Created")}: {guild.CreatedAt:yyyy-MM-dd}",
                $"{MessageTextFormat.Bold("Owner")}: {MessageTextFormat.Mention(guild.OwnerId)}"
            };

            var statistics = new[]
            {
                $"{MessageTextFormat.Bold("Members")}: {guild.MemberCount}",
                $"{MessageTextFormat.Bold("Emotes")}: {guild.Emotes.Count}",
                $"{MessageTextFormat.Bold("Roles")}: {guild.Roles.Count}",
                $"{MessageTextFormat.Bold("Channels")}: {guild.Channels.Count}"
            };

            var data = new[]
            {
                $"{MessageTextFormat.Bold("Max Bitrate")}: {guild.MaxBitrate}",
                $"{MessageTextFormat.Bold("Max Upload Size")}: {guild.MaxUploadLimit}",
                $"{MessageTextFormat.Bold("Max Members")}: {guild.MaxMembers}"
            };

            var embed = new EmbedBuilder();

            embed.AddField(new EmbedFieldBuilder()
                .WithName(MessageTextFormat.Bold("General Information"))
                .WithValue(String.Join("\n", generalInfo))
                .WithIsInline(true));

            embed.AddField(new EmbedFieldBuilder()
                .WithName(MessageTextFormat.Bold("Server Statistics"))
                .WithValue(String.Join("\n", statistics))
                .WithIsInline(true));

            embed.AddField(new EmbedFieldBuilder()
                .WithName("Data")
                .WithValue(String.Join("\n", data))
                .WithIsInline(true));

            await RespondAsync(embed: embed.Build());
        }
    }
}
