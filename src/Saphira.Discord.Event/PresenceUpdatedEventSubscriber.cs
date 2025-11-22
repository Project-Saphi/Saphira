using Discord;
using Discord.WebSocket;
using Saphira.Core.Application;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Guild.Role;
using Saphira.Discord.Logging;
using Saphira.Discord.Messaging;
using Saphira.Discord.Presence;
using Saphira.Util.Twitch;

namespace Saphira.Discord.Event;

[AutoRegister]
public class PresenceUpdatedEventSubscriber(DiscordSocketClient client, Configuration configuration, TwitchClient twitchClient, IMessageLogger logger) : IDiscordSocketClientEventSubscriber
{
    private bool _isRegistered = false;

    public void Register()
    {
        if (_isRegistered) return;

        client.PresenceUpdated += HandlePresenceUpdatedAsync;
        _isRegistered = true;
    }

    public void Unregister()
    {
        if (!_isRegistered) return;

        client.PresenceUpdated -= HandlePresenceUpdatedAsync;
        _isRegistered = false;
    }

    private async Task HandlePresenceUpdatedAsync(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        if (user is not SocketGuildUser guildUser)
        {
            return;
        }

        await PostLivestream(guildUser, oldPresence, newPresence);
        await ToggleStreamingRole(guildUser, newPresence);
    }

    private async Task PostLivestream(SocketGuildUser guildUser, SocketPresence oldPresence, SocketPresence newPresence)
    {
        var channel = guildUser.Guild.Channels.Where(c => c.Name == configuration.LivestreamsChannel).FirstOrDefault();

        if (channel == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"No channel {configuration.LivestreamsChannel} exists. Unable to post new livestreams.");
            return;
        }

        if (channel is not SocketTextChannel livestreamsChannel)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"The channel {configuration.LivestreamsChannel} is not a text channel. Unable to post new livestreams.");
            return;
        }

        var isNewStream = !(oldPresence.Activities?.Where(a => Activity.IsCTRStream(a)).Any() ?? false);
        var streamActivity = newPresence.Activities?.Where(a => Activity.IsCTRStream(a)).FirstOrDefault();

        if (!isNewStream || streamActivity == null || streamActivity is not StreamingGame game)
        {
            return;
        }

        var streamingPlatform = Activity.GetStreamingPlatform(streamActivity.Name);
        var embed = GetEmbedForStream(guildUser, streamActivity, game);

        FileAttachment? attachment = null;

        if (streamingPlatform == Activity.StreamingPlatform.Twitch)
        {
            var accountName = Twitch.ExtractAccountNameFromStreamUrl(game.Url);
            var imageStream = await twitchClient.FetchStreamPreview(accountName ?? "");

            if (imageStream != null)
            {
                attachment = new FileAttachment(imageStream, "thumbnail.jpg");
                embed.WithThumbnailUrl($"attachment://{attachment.Value.FileName}");
            }
        }

        if (attachment != null)
        {
            // For some reason SendMessageAsync has no attachments parameter lmao
            await livestreamsChannel.SendFileAsync(attachment.Value, embed: embed.Build());
            return;
        }

        await livestreamsChannel.SendMessageAsync(embed: embed.Build());
    }

    private EmbedBuilder GetEmbedForStream(SocketGuildUser guildUser, IActivity streamActivity, StreamingGame game)
    {
        var streamingPlatform = Activity.GetStreamingPlatform(streamActivity.Name);

        var data = new[]
        {
            $"{MessageTextFormat.Bold("Streamer")}: {guildUser.Mention}",
            $"{MessageTextFormat.Bold("Title")}: `{streamActivity.Details}`",
            $"{MessageTextFormat.Bold("Game")}: {game.Name}",
            $"{MessageTextFormat.Bold("Channel")}: {MessageTextFormat.MaskedLink(game.Url, game.Url)}",
        };

        var embed = new EmbedBuilder()
            .WithAuthor($"New Livestream on {streamActivity.Name}", url: game.Url)
            .WithColor(streamingPlatform.HasValue ? (uint) streamingPlatform.Value : Color.Default)
            .WithUrl(game.Url)
            .WithTimestamp(DateTimeOffset.Now);

        var detailsField = new EmbedFieldBuilder()
            .WithName(":receipt: Details")
            .WithValue(String.Join('\n', data));

        embed.AddField(detailsField);

        return embed;
    }

    private async Task ToggleStreamingRole(SocketGuildUser guildUser, SocketPresence presence)
    {
        var streamingRole = guildUser.Guild.Roles.Where(r => GuildRole.IsStreamingRole(r)).FirstOrDefault();

        if (streamingRole == null)
        {
            return;
        }

        var isStreaming = presence.Activities?.Where(a => Activity.IsCTRStream(a)).Any() ?? false;
        var hasStreamingRole = guildUser.Roles.Contains(streamingRole);

        if (isStreaming && !hasStreamingRole)
        {
            await guildUser.AddRoleAsync(streamingRole);
        }

        if (!isStreaming && hasStreamingRole)
        {
            await guildUser.RemoveRoleAsync(streamingRole);
        }
    }
}
