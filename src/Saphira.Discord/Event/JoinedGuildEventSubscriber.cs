using Discord.WebSocket;
using Saphira.Core;
using Saphira.Core.Event;
using Saphira.Core.Extensions.DependencyInjection;

namespace Saphira.Discord.Event;

[AutoRegister]
public class JoinedGuildEventSubscriber(DiscordSocketClient client, Configuration configuration) : IEventSubscriber
{
    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.JoinedGuild += HandleGuildJoinedAsync;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.JoinedGuild -= HandleGuildJoinedAsync;
        _isRegistered = false;
    }

    private async Task HandleGuildJoinedAsync(SocketGuild guild)
    {
        var mainChannel = guild.Channels.FirstOrDefault(channel => channel.Name == configuration.MainChannel);

        if (mainChannel is not SocketTextChannel textChannel)
        {
            return;
        }

        await textChannel.SendMessageAsync("I am here. :slight_smile:");
    }
}
