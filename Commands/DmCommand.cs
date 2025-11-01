using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
[RequireTeamMemberRole]
public class DmCommand : InteractionModuleBase<SocketInteractionContext>
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("dm", "DM someone as Saphira")]
    public async Task HandleCommand(string message, SocketGuildUser user)
    {
        await DeferAsync();

        try
        {
            await user.SendMessageAsync(message);

            var successAlert = new SuccessAlertEmbedBuilder($"Successfully sent DM to {user.Mention}.");
            await FollowupAsync(embed: successAlert.Build());
        }
        catch (Exception ex)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to send DM: {ex.Message}");
            await FollowupAsync(embed: errorAlert.Build());
        }
    }
}
