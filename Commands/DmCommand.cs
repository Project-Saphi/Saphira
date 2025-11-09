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
public class DmCommand(IMessageLogger logger) : BaseCommand
{
    public override CommandMetadata GetMetadata()
    {
        return new CommandMetadata(
            "DM someone as Saphira",
            "/dm @Garma",
            "The player must be a member of the server"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("dm", "DM someone as Saphira")]
    public async Task HandleCommand([MaxLength(2000)] string message, SocketGuildUser user)
    {
        await DeferAsync();

        try
        {
            await user.SendMessageAsync(message);

            var successAlert = new SuccessAlertEmbedBuilder($"Successfully sent DM to {user.Mention}.");
            await FollowupAsync(embed: successAlert.Build());

            logger.Log(LogSeverity.Info, Context.User.Username, $"DM send to {user.Username} ({user.Id}): {message}");
        }
        catch (Exception ex)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to send DM: {ex.Message}");
            await FollowupAsync(embed: errorAlert.Build());
        }
    }
}
