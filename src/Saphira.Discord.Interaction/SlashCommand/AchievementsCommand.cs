using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Foundation.Autocompletion;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Saphi.Api;
using Saphira.Saphi.Interaction;

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

        if (!userProfileResult.Success || userProfileResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve user profile: {userProfileResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var stats = userProfileResult.Response.Data.Stats;
        var country = userProfileResult.Response.Data.Country;

        var content = new[]
        {
            $"{MessageTextFormat.Bold("Total Points")}: {stats.TotalPoints}",
            $"{MessageTextFormat.Bold("Total Time")}: {stats.TotalTimeFormatted}",
            $"{MessageTextFormat.Bold("First Places")}: {stats.FirstPlaces}",
            $"{MessageTextFormat.Bold("Podium Finishes")}: {stats.PodiumFinishes}",
            $"{MessageTextFormat.Bold("Tracks Submitted")}: {stats.TracksSubmitted}",
            $"{MessageTextFormat.Bold("Average Finish")}: {stats.AvgFinish:F3}",
            $"{MessageTextFormat.Bold("Average Rank")}: {stats.AvgStandard:F3}"
        };

        var embed = new EmbedBuilder();
        
        var field = new EmbedFieldBuilder()
            .WithName($"{CountryEmoteMapper.MapCountryToEmote(country.Name)} {userProfileResult.Response.Data.Username}'s Achievements")
            .WithValue(string.Join("\n", content));

        embed.AddField(field);

        await FollowupAsync(embed: embed.Build());
    }
}
