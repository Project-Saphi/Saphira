using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Metadata;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Util.Logging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
[RequireTeamMemberRole]
public class PostCommand(IMessageLogger logger) : BaseCommand
{
    public override CommandMetadata GetMetadata()
    {
        return new CommandMetadata(
            "/post Hello Guys! #general",
            "Limited to text channels - Discord message length restrictions apply as usual"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("post", "Send a message as Saphira")]
    public async Task HandleCommand([MaxLength(2000)] string message, SocketChannel channel)
    {
        await DeferAsync();

        if (channel is not SocketTextChannel textChannel)
        {
            var errorAlert = new ErrorAlertEmbedBuilder("The specified channel is not a text channel.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        await textChannel.SendMessageAsync(message);

        var successAlert = new SuccessAlertEmbedBuilder("Message has been sent successfully.");
        await FollowupAsync(embed: successAlert.Build());

        logger.Log(LogSeverity.Info, Context.User.Username, $"Message sent as Saphira in channel {textChannel.Name} ({textChannel.Id}): {message}");
    }
}
