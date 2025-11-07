using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class ServerCommand : InteractionModuleBase<SocketInteractionContext>
{
    [CommandContextType(InteractionContextType.Guild)]
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

        var embed = new EmbedBuilder()
            .WithAuthor("Server Data");

        AddEmbedField(embed, ":desktop:", "Information", generalInfo);
        AddEmbedField(embed, ":bar_chart:", "Statistics", statistics);
        AddEmbedField(embed, ":level_slider:", "Metrics", data);

        await RespondAsync(embed: embed.Build());
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, string[] content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }
}
