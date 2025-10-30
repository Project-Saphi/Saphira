using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class KickCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("kick", "Kick a user from the server")]
        public async Task HandleCommand(SocketGuildUser user, string reason = "No reason provided")
        {
            await DeferAsync();

            if (user.Id == Context.User.Id)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("You cannot kick yourself.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            if (user.Id == Context.Client.CurrentUser.Id)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("I cannot kick myself.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            try
            {
                await user.KickAsync(reason);

                var successAlert = new SuccessAlertEmbedBuilder($"Successfully kicked {MessageTextFormat.Bold(user.Username)}.\nReason: {reason}");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to kick user: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
