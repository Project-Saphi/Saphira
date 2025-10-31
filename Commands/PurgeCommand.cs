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
    public class PurgeCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [SlashCommand("purge", "Delete the last X messages in the current channel")]
        public async Task HandleCommand(
            [MinValue(1)]
            [MaxValue(100)]
            int count)
        {
            await DeferAsync();

            if (Context.Channel is not SocketTextChannel textChannel)
            {
                return;
            }

            var messages = await textChannel.GetMessagesAsync(count).FlattenAsync();

            // Discord only allows mass deleting messages that are less than 14 days old because ... Discord
            // There is also a limitation that you cannot delete more than 500 messages at once but that's more than we need lmao
            var messagesToDelete = messages.Where(m => (DateTimeOffset.UtcNow - m.Timestamp).TotalDays < 14).ToList();

            if (messagesToDelete.Count == 0)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("No messages found to delete (messages must be less than 14 days old).");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            try
            {
                await textChannel.DeleteMessagesAsync(messagesToDelete);
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to delete messages: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
