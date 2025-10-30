using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class ReactCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("react", "React to a message as Saphira")]
        public async Task HandleCommand(IEmote emote, string messageId)
        {
            await DeferAsync();

            if (Context.Channel is not SocketTextChannel textChannel)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("This command can only be used in text channels!");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            if (!ulong.TryParse(messageId, out ulong parsedMessageId))
            {
                var errorAlert = new ErrorAlertEmbedBuilder("Invalid message ID format.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            var message = await textChannel.GetMessageAsync(parsedMessageId);

            if (message == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("There exists no message with that ID.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            try
            {
                await message.AddReactionAsync(emote);

                var successAlert = new SuccessAlertEmbedBuilder($"Successfully reacted to message {parsedMessageId} with {emote.Name}");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to add reaction: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
