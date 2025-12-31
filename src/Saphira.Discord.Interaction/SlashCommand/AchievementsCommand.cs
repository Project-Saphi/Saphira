using Discord;
using Discord.Interactions;
using Saphira.Discord.Core.Interaction.Autocompletion;
using Saphira.Discord.Core.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Saphi.Api;
using Saphira.Saphi.Core.Entity.User;
using Saphira.Saphi.Interaction.Autocompletion.ValueProvider;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class AchievementsCommand(ISaphiApiClient client) : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata("/achievements Niikasd");
    }

    [SlashCommand("achievements", "Show a player's achievements")]
    public async Task HandleCommand(
        [Autocomplete(typeof(GenericAutocompleteHandler<PlayerValueProvider>))] int player
        )
    {
        await DeferAsync();

        var userProfileResult = await client.GetUserProfileAsync(player);

        if (!userProfileResult.Success || userProfileResult.Response == null || userProfileResult.Response.Data.Count == 0)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve user profile: {userProfileResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var userProfile = userProfileResult.Response.Data.First();
        var stats = userProfile.Stats;
        var country = userProfile.Country;

        var performanceMetrics = new[]
        {
            $"{MessageTextFormat.Bold("First Places")}: {stats.FirstPlaces}",
            $"{MessageTextFormat.Bold("Podiums")}: {stats.PodiumFinishes}",
            $"{MessageTextFormat.Bold("Total Points")}: {stats.TotalPoints}",
            $"{MessageTextFormat.Bold("Total Points")}: {stats.TotalTimeFormatted}",
            $"{MessageTextFormat.Bold("Average Finish")}: {stats.AverageFinish:F3}",
            $"{MessageTextFormat.Bold("Average Rank")}: {stats.AverageStandard:F3}",
            $"{MessageTextFormat.Bold("Average SR:PR")}: {stats.AverageSrPr:F3}",
        };

        var statistics = new[]
        {
            $"{MessageTextFormat.Bold("Total Submissions")}: {stats.TracksSubmitted}",
            $"{MessageTextFormat.Bold("Completed Tracks")}: {stats.CompletedTracks} / {stats.TotalTracks}",
            $"{MessageTextFormat.Bold("First Submission")}: {FormatDate(stats.FirstSubmissionAt)}",
            $"{MessageTextFormat.Bold("Last Submission")}: {FormatDate(stats.LastSubmissionAt)}",
            $"{MessageTextFormat.Bold("Most Played Track")}: {FormatMostPlayedTrack(stats.MostPlayedTrack)}",
            $"{MessageTextFormat.Bold("Most Played Character")}: {FormatMostPlayedCharacter(stats.MostPlayedCharacter)}",
            $"{MessageTextFormat.Bold("Most Played Engine")}: {FormatMostPlayedEngine(stats.MostPlayedEngine)}"
        };

        var embed = new EmbedBuilder()
            .WithAuthor($"{userProfile.Name}'s Achievements");

        embed.AddField(":trophy: Performance", string.Join("\n", performanceMetrics), true);
        embed.AddField(":bar_chart: Statistics", string.Join("\n", statistics), true);

        await FollowupAsync(embed: embed.Build());
    }

    private static string FormatDate(string? date)
    {
        if (string.IsNullOrEmpty(date))
            return "-";

        if (DateTime.TryParse(date, out var parsed))
            return parsed.ToString("yyyy-MM-dd");

        return date;
    }

    private static string FormatMostPlayedTrack(MostPlayedItem? item)
    {
        if (item == null)
            return "-";

        return $"{item.Name} ({item.SubmissionCount})";
    }

    private static string FormatMostPlayedCharacter(MostPlayedItem? item)
    {
        if (item == null)
            return "-";

        var emote = CharacterEmoteMapper.MapCharacterToEmote(item.Name);
        return $"{emote} ({item.SubmissionCount})";
    }

    private static string FormatMostPlayedEngine(MostPlayedItem? item)
    {
        if (item == null)
            return "-";

        var emote = EngineEmoteMapper.MapEngineToEmote(item.Name);
        return $"{emote} ({item.SubmissionCount})";
    }
}
