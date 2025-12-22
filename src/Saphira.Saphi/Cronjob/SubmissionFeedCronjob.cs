using Discord;
using Discord.WebSocket;
using Saphira.Core;
using Saphira.Core.Cronjob;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Cronjob;

[AutoRegister]
public class SubmissionFeedCronjob(DiscordSocketClient discordClient, ISaphiApiClient saphiClient, Configuration configuration, IMessageLogger logger) : ICronjob
{
    public async Task ExecuteAsync()
    {
        var guild = discordClient.Guilds.FirstOrDefault(g => g.Id == configuration.GuildId);

        if (guild == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Saphira is not in the server with ID {configuration.GuildId}. Unable to post new submissions");
            return;
        }

        var channel = guild.Channels.FirstOrDefault(c => c.Name == configuration.SubmissionFeedChannel);

        if (channel == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"No channel {configuration.SubmissionFeedChannel} exists. Unable to post new submissions");
            return;
        }

        if (channel is not SocketTextChannel textChannel)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"The channel {configuration.SubmissionFeedChannel} is not a text channel. Unable to post new submissions");
            return;
        }

        var result = await saphiClient.GetRecentSubmissionsAsync("1m", forceRefresh: true);

        if (!result.Success || result.Response == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Unable to fetch recent submissions: {result.ErrorMessage}");
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            logger.Log(LogSeverity.Verbose, "Saphira", "No new submissions found.");
            return;
        }

        logger.Log(LogSeverity.Verbose, "Saphira", $"Found {result.Response.Data.Count} new submission(s) to post");

        var submissions = result.Response.Data;
        submissions.Reverse();

        foreach (var submission in submissions)
        {
            var submissionEmbed = GetEmbedForSubmission(submission);
            await textChannel.SendMessageAsync(embed: submissionEmbed.Build());
        }
    }

    private EmbedBuilder GetEmbedForSubmission(RecentSubmission submission)
    {
        var data = new[]
        {
            $"{MessageTextFormat.Bold("Track")}: {submission.TrackName}",
            $"{MessageTextFormat.Bold("Category")}: {submission.CategoryName}",
            $"{MessageTextFormat.Bold("Time")}: {submission.TimeFormatted}",
            $"{MessageTextFormat.Bold("Character")}: {CharacterEmoteMapper.MapCharacterToEmote(submission.CharacterName)}",
            $"{MessageTextFormat.Bold("Engine")}: {EngineEmoteMapper.MapEngineToEmote(submission.EngineName)}",
            $"{MessageTextFormat.Bold("Country")}: {CountryEmoteMapper.MapCountryToEmote(submission.CountryName)}"
        };

        var embed = new EmbedBuilder()
            .WithThumbnailUrl("https://i.imgur.com/esMgq3Y.png")
            .WithTimestamp(DateTimeOffset.Now);

        if (submission.IsWorldRecord)
        {
            embed
                .WithColor(14530399)
                .WithAuthor($"New world record by {submission.Name}");
        }
        else
        {
            embed
                .WithColor(5526696)
                .WithAuthor($"New submission by {submission.Name}");
        }

        var dataField = new EmbedFieldBuilder()
            .WithName($":receipt: {MessageTextFormat.Bold("Details")}")
            .WithValue(string.Join("\n", data));

        embed.AddField(dataField);

        return embed;
    }

    public TimeSpan GetStartDelay()
    {
        return GetInterval();
    }

    public TimeSpan GetInterval()
    {
        return TimeSpan.FromMinutes(1);
    }
}
