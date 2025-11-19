using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Autocompletion;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Util.EmoteMapper;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class AchievementsCommand(CachedClient client) : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata("/achievements Niikasd");
    }

    [SlashCommand("achievements", "Show a player's achievements")]
    public async Task HandleCommand(
        [Autocomplete(typeof(PlayerAutocompleteHandler))] string player
        )
    {
        var result = await client.GetUserProfileAsync(player);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve user profile: {result.ErrorMessage ?? "Unknown error"}");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        var stats = result.Response.Data.Stats;
        var country = result.Response.Data.Country;

        var content = new[]
        {
            $"{MessageTextFormat.Bold("Total Points")}: {stats.TotalPoints}",
            $"{MessageTextFormat.Bold("Course Points")}: {stats.CoursePoints}",
            $"{MessageTextFormat.Bold("Lap Points")}: {stats.LapPoints}",
            $"{MessageTextFormat.Bold("First Places")}: {stats.FirstPlaces}",
            $"{MessageTextFormat.Bold("Podium Finishes")}: {stats.PodiumFinishes}"
        };

        var embed = new EmbedBuilder();
        
        var field = new EmbedFieldBuilder()
            .WithName($"{CountryEmoteMapper.MapCountryToEmote(country.Name)} {result.Response.Data.Username}'s Achievements")
            .WithValue(string.Join("\n", content));

        embed.AddField(field);

        await RespondAsync(embed: embed.Build());
    }
}
