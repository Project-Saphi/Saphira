using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class BanCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("ban", "Ban a user from the server")]
        public async Task HandleCommand(
            SocketGuildUser user,
            string reason = "No reason provided",
            [MinValue(0)]
            [MaxValue(7)]
            int deleteMessageDays = 0)
        {
            await DeferAsync();

            if (user.Id == Context.User.Id)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("You cannot ban yourself.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            if (user.Id == Context.Client.CurrentUser.Id)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("I cannot ban myself.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            try
            {
                await Context.Guild.AddBanAsync(user, deleteMessageDays, reason);

                var successAlert = new SuccessAlertEmbedBuilder($"Successfully banned {MessageTextFormat.Bold(user.Username)}.\nReason: {reason}\nDeleted messages from last {deleteMessageDays} day(s).");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to ban user: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
