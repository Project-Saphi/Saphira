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

        private bool _isRegistered = false;

        public MessageReceivedEventSubscriber(DiscordSocketClient client, InviteLinkDetector inviteLinkDetector)
        {
            _client = client;
            _inviteLinkDetector = inviteLinkDetector;
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

            if (_inviteLinkDetector.MessageContainsInviteLink(message.Content))
            {
                await message.DeleteAsync();

                var warningAlert = new AlertEmbedBuilder($"{message.Author.Mention}, posting discord invite links is not allowed.");
                await textChannel.SendMessageAsync(embed: warningAlert.Build());
            }
        }
    }
}
