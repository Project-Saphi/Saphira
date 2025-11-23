using Discord;
using Discord.WebSocket;
using Saphira.Core;
using Saphira.Core.Event;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Discord.Entity.Guild.Member;
using Saphira.Discord.Messaging;

namespace Saphira.Discord.Event;

[AutoRegister]
public class MessageReceivedEventSubscriber(
    DiscordSocketClient client, 
    Configuration configuration, 
    InviteLinkDetector inviteLinkDetector, 
    RestrictedContentDetector restrictedContentDetector,
    IMessageLogger logger
    ) : IEventSubscriber
{
    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.MessageReceived += HandleMessageReceivedAsync;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.MessageReceived -= HandleMessageReceivedAsync;
        _isRegistered = false;
    }

    private async Task HandleMessageReceivedAsync(SocketMessage message)
    {
        await SuppressLivestreamEmbeds(message);

        if (message.Author.IsBot || GuildMember.IsTeamMember(message.Author))
        {
            return;
        }

        await RemoveDiscordInviteLinks(message);
        await RemoveRestrictedContent(message);
    }

    private async Task RemoveDiscordInviteLinks(SocketMessage message)
    {
        if (message.Channel is not SocketTextChannel textChannel)
        {
            return;
        }

        if (inviteLinkDetector.MessageContainsInviteLink(message.Content))
        {
            await message.DeleteAsync();

            var warningAlert = new WarningAlertEmbedBuilder($"{message.Author.Mention}, posting discord invite links is not allowed.");
            await textChannel.SendMessageAsync(embed: warningAlert.Build());

            logger.Log(LogSeverity.Verbose, "Saphira", $"Deleted message {message.Id} from {message.Author.GlobalName} ({message.Author.Id}) due to containing a Discord invite link");
        }
    }

    private async Task RemoveRestrictedContent(SocketMessage message)
    {
        if (message.Channel is not SocketTextChannel textChannel || message.Author is not SocketGuildUser guildUser)
        {
            return;
        }

        if (!GuildMember.IsVerified(guildUser) && GuildMember.IsNewUser(guildUser) && restrictedContentDetector.MessageContainsRestrictedContent(message))
        {
            await message.DeleteAsync();

            var warningAlert = new WarningAlertEmbedBuilder($"{message.Author.Mention}, new members cannot post images, links, attachments, or videos until they have been on the server for at least 12 hours.");
            await textChannel.SendMessageAsync(embed: warningAlert.Build());

            logger.Log(LogSeverity.Verbose, "Saphira", $"Deleted message {message.Id} from {message.Author.GlobalName} ({message.Author.Id}) due to containing media.");
        }
    }

    private async Task SuppressLivestreamEmbeds(SocketMessage message)
    {
        if (message.Channel is not SocketTextChannel textChannel)
        {
            return;
        }

        if (textChannel.Name == configuration.LivestreamsChannel && message is IUserMessage userMessage)
        {
            await userMessage.ModifyAsync(m =>
            {
                m.Flags = MessageFlags.SuppressEmbeds;
            });

            logger.Log(LogSeverity.Verbose, "Saphira", $"Suppressed embeds for message {message.Id}");
        }
    }
}
