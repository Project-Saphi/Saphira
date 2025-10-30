using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class TimeoutCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("timeout", "Timeout a user for a specified duration")]
        public async Task HandleCommand(
            SocketGuildUser user,
            [MinValue(1)]
            [MaxValue(40320)]
            int minutes,
            string reason = "No reason provided")
        {
            await DeferAsync();

            if (user.Id == Context.User.Id)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("You cannot timeout yourself.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            if (user.Id == Context.Client.CurrentUser.Id)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("I cannot timeout myself.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            try
            {
                var timeoutDuration = TimeSpan.FromMinutes(minutes);
                await user.SetTimeOutAsync(timeoutDuration, new RequestOptions { AuditLogReason = reason });

                var successAlert = new SuccessAlertEmbedBuilder($"Successfully timed out {MessageTextFormat.Bold(user.Username)} for {MessageTextFormat.Bold(minutes.ToString())} minute(s).\nReason: {reason}");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to timeout user: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
