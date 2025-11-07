using Discord.WebSocket;
using Saphira.Extensions.DependencyInjection;

namespace Saphira.Discord.EventSubscriber;

[AutoRegister]
public class UserJoinedEventSubscriber(DiscordSocketClient client, BotConfiguration configuration) : IDiscordSocketClientEventSubscriber
{
    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.UserJoined += HandleUserJoinedAsync;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.UserJoined -= HandleUserJoinedAsync;
        _isRegistered = false;
    }

    private async Task HandleUserJoinedAsync(SocketGuildUser user)
    {
        var guild = user.Guild;

        if (guild.Users.Count % 100 == 0)
        {
            var mainChannel = guild.Channels.FirstOrDefault(channel => channel.Name == configuration.MainChannel);
            if (mainChannel is not SocketTextChannel textChannel)
            {
                return;
            }

            await textChannel.SendMessageAsync($"We just reached {guild.Users.Count} members. ??");
            return;
        }
    }
}
