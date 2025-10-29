using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class ModeratorCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("dm", "DM someone as Saphira")]
        public async Task DmCommand(string message, SocketGuildUser user)
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

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("post", "Send a message as Saphira")]
        public async Task PostCommand(string message, SocketChannel channel)
        {
            await DeferAsync();

            if (channel is not SocketTextChannel textChannel)
            {
                var errorAlert = new ErrorAlertEmbedBuilder("The specified channel is not a text channel!");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            await textChannel.SendMessageAsync(message);

            var successAlert = new SuccessAlertEmbedBuilder("Message has been sent successfully.");
            await FollowupAsync(embed: successAlert.Build());
        }

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("purge", "Delete the last X messages in the current channel")]
        public async Task PurgeCommand(
            [MinValue(1)]
            [MaxValue(100)]
            int count)
        {
            await DeferAsync();

            var textChannel = Context.Channel as SocketTextChannel;
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

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("react", "React to a message as Saphira")]
        public async Task ReactCommand(IEmote emote, string messageId)
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

                var successAlert = new SuccessAlertEmbedBuilder($"Successfully reacted to message with {emote.Name}");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to add reaction: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("verify", "Verify a user")]
        public async Task VerifyCommand(SocketGuildUser user)
        {
            await DeferAsync();

            var verifiedRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == Discord.Guild.GuildRole.VerifiedRole);

            if (verifiedRole == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"The {MessageTextFormat.Bold(Discord.Guild.GuildRole.VerifiedRole)} role does not exist on this server.");
                await FollowupAsync(embed: errorAlert.Build());
                return;
            }

            if (user.Roles.Contains(verifiedRole))
            {
                var infoAlert = new InfoAlertEmbedBuilder($"{user.Mention} is already verified.");
                await FollowupAsync(embed: infoAlert.Build());
                return;
            }

            try
            {
                await user.AddRoleAsync(verifiedRole);

                var successAlert = new SuccessAlertEmbedBuilder($"Successfully verified {user.Mention}.");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to verify user: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
