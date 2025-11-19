using Discord.WebSocket;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;
using Saphira.Discord.Extensions.DependencyInjection;

namespace Saphira.Discord.EventSubscriber;

[AutoRegister]
public class MessageReceivedEventSubscriber(DiscordSocketClient client, InviteLinkDetector inviteLinkDetector, RestrictedContentDetector restrictedContentDetector) : IDiscordSocketClientEventSubscriber
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
        if (message.Author.IsBot || GuildMember.IsTeamMember(message.Author))
            return;

        if (message.Channel is not SocketTextChannel textChannel ||
            message.Author is not SocketGuildUser guildUser)
            return;

        if (inviteLinkDetector.MessageContainsInviteLink(message.Content))
        {
            await message.DeleteAsync();

            var warningAlert = new WarningAlertEmbedBuilder($"{message.Author.Mention}, posting discord invite links is not allowed.");
            await textChannel.SendMessageAsync(embed: warningAlert.Build());
            return;
        }

        if (!GuildMember.IsVerified(guildUser) && GuildMember.IsNewUser(guildUser) && restrictedContentDetector.MessageContainsRestrictedContent(message))
        {
            await message.DeleteAsync();

            var warningAlert = new WarningAlertEmbedBuilder(
                $"{message.Author.Mention}, new members cannot post images, links, attachments, or videos until they have been on the server for at least 12 hours."
            );
            await textChannel.SendMessageAsync(embed: warningAlert.Build());
            return;
        }
    }
}
