using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class ServerCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [SlashCommand("server", "Get information about this server")]
        public async Task HandleCommand()
        {
            var guild = Context.Guild;

            if (guild == null)
            {
                await RespondAsync("This command can only be used in a server!");
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
