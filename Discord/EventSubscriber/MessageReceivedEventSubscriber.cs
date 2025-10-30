using Discord.WebSocket;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;
using Saphira.Extensions.DependencyInjection;

namespace Saphira.Discord.EventSubscriber
{
    [AutoRegister]
    public class MessageReceivedEventSubscriber : IDiscordSocketClientEventSubscriber
    {
        private readonly DiscordSocketClient _client;
        private readonly InviteLinkDetector _inviteLinkDetector;
        private readonly RestrictedContentDetector _restrictedContentDetector;

        private bool _isRegistered = false;

        public MessageReceivedEventSubscriber(DiscordSocketClient client, InviteLinkDetector inviteLinkDetector, RestrictedContentDetector restrictedContentDetector)
        {
            _client = client;
            _inviteLinkDetector = inviteLinkDetector;
            _restrictedContentDetector = restrictedContentDetector;
        }

        public void Register()
        {
            if (_isRegistered) return;

            _client.MessageReceived += HandleMessageReceivedAsync;
            _isRegistered = true;
        }

        public void Unregister()
        {
            if (!_isRegistered) return;

            _client.MessageReceived -= HandleMessageReceivedAsync;
            _isRegistered = false;
        }

        private async Task HandleMessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot || GuildMember.IsTeamMember(message.Author))
            {
                return;
            }

            if (message.Channel is not SocketTextChannel textChannel)
            {
                return;
            }

            if (message.Author is not SocketGuildUser guildUser)
            {
                return;
            }

            // None of the checks below apply to team members
            if (GuildMember.IsTeamMember(guildUser))
            {
                return;
            }

            if (_inviteLinkDetector.MessageContainsInviteLink(message.Content))
            {
                await message.DeleteAsync();

                var warningAlert = new WarningAlertEmbedBuilder($"{message.Author.Mention}, posting discord invite links is not allowed.");
                await textChannel.SendMessageAsync(embed: warningAlert.Build());
                return;
            }

            if (!GuildMember.IsVerified(guildUser) == false && GuildMember.IsNewUser(guildUser) && _restrictedContentDetector.MessageContainsRestrictedContent(message))
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
}
