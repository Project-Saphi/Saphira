using Discord.WebSocket;
using Saphira.Extensions.DependencyInjection;

namespace Saphira.Discord.EventSubscriber;

[AutoRegister]
public class JoinedGuildEventSubscriber(DiscordSocketClient client, Configuration configuration) : IDiscordSocketClientEventSubscriber
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
        return;
    }
}
