using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    [RequireTeamMemberRole]
    public class PostCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [SlashCommand("post", "Send a message as Saphira")]
        public async Task HandleCommand(string message, SocketChannel channel)
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
        }
    }
}
