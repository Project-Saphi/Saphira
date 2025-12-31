using Discord;
using Discord.WebSocket;
using Saphira.Core;
using Saphira.Core.Event;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Core.Twitch;
using Saphira.Discord.Core.Entity.Guild.Role;
using Saphira.Discord.Core.Entity.Presence;
using Saphira.Discord.Messaging;

namespace Saphira.Discord.Event;

[AutoRegister]
public class PresenceUpdatedEventSubscriber(DiscordSocketClient client, Configuration configuration, TwitchClient twitchClient, IMessageLogger logger) : IEventSubscriber
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
        var channel = guildUser.Guild.Channels.FirstOrDefault(c => c.Name == configuration.LivestreamsChannel);

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

        var isNewStream = !(oldPresence.Activities?.Any(a => Activity.IsCTRStream(a)) ?? false);
        var streamActivity = newPresence.Activities?.FirstOrDefault(a => Activity.IsCTRStream(a));

        if (!isNewStream || streamActivity == null || streamActivity is not StreamingGame game)
        {
            return;
        }

        var embed = GetEmbedForStream(guildUser, streamActivity, game);

        FileAttachment? attachment = null;
        var streamingPlatform = Activity.GetStreamingPlatform(streamActivity.Name);

        if (streamingPlatform == StreamingPlatform.Twitch)
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
            $"{MessageTextFormat.Bold("Title")}: ```{streamActivity.Details}```",
            $"{MessageTextFormat.Bold("Game")}: Crash Team Racing",
            $"{MessageTextFormat.Bold("Channel")}: {game.Url}",
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
            logger.Log(LogSeverity.Error, "Saphira", $"The role '{GuildRole.StreamingRole}' does not exist. Unable to assign to users who are streaming.");
            return;
        }

        var isStreaming = presence.Activities?.Any(a => Activity.IsCTRStream(a)) ?? false;
        var hasStreamingRole = guildUser.Roles.Contains(streamingRole);

        if (isStreaming && !hasStreamingRole)
        {
            await guildUser.AddRoleAsync(streamingRole);
            logger.Log(LogSeverity.Verbose, "Saphira", $"Added role '{streamingRole.Name}' to {guildUser.GlobalName} ({guildUser.Id})");
        }

        if (!isStreaming && hasStreamingRole)
        {
            await guildUser.RemoveRoleAsync(streamingRole);
            logger.Log(LogSeverity.Verbose, "Saphira", $"Removed role '{streamingRole.Name}' from {guildUser.GlobalName} ({guildUser.Id})");
        }
    }
}
