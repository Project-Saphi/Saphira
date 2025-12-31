using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Core.Logging;
using Saphira.Discord.Core.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
[RequireTeamMemberRole]
public class ReactCommand(IMessageLogger logger) : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/react :flag_de: 1437024317857726505",
            "Saphira must have access to the channel where the message is"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
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

            logger.Log(LogSeverity.Info, Context.User.Username, $"Reacted to message {parsedMessageId} with emote {emote.Name}");
        }
        catch (Exception ex)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to add reaction: {ex.Message}");
            await FollowupAsync(embed: errorAlert.Build());
        }
    }
}
