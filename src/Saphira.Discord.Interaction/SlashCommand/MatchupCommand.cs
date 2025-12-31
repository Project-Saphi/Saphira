using Discord;
using Discord.Interactions;
using Saphira.Discord.Core.Interaction.Autocompletion;
using Saphira.Discord.Core.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Saphi.Api;
using Saphira.Saphi.Core.Entity;
using Saphira.Saphi.Interaction.Autocompletion.ValueProvider;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class MatchupCommand(ISaphiApiClient client) : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/matchup Niikasd Garma Course",
            "Players must have at least 1 track in common that they've played"
        );
    }

    [SlashCommand("matchup", "Show the matchup between 2 players for a category")]
    public async Task HandleCommand(
        [Autocomplete(typeof(GenericAutocompleteHandler<PlayerValueProvider>))] int player1,
        [Autocomplete(typeof(GenericAutocompleteHandler<PlayerValueProvider>))] int player2,
        [Autocomplete(typeof(GenericAutocompleteHandler<CategoryValueProvider>))] int category
        )
    {
        await DeferAsync();

        if (player1 == player2)
        {
            var errorAlert = new ErrorAlertEmbedBuilder("You cannot compare a player against himself.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var result = await client.GetMatchupAsync(player1, player2, category.ToString());

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder(result.ErrorMessage ?? "An error occurred fetching the matchup.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var errorAlert = new ErrorAlertEmbedBuilder("One or more players don't exist.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var matchup = result.Response.Data.First();

        if (matchup.Comparisons.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("These players have no tracks in common.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var matchupEmbed = GetMatchupEmbed(matchup);
        await FollowupAsync(embed: matchupEmbed.Build());
    }

    private EmbedBuilder GetMatchupEmbed(MatchupResult matchup)
    {
        var categoryName = matchup.Comparisons.First().CategoryName;

        var embed = new EmbedBuilder()
            .WithAuthor($"Matchup between {matchup.Player1.Name} and {matchup.Player2.Name} ({categoryName})");

        if (matchup.OverallWinner.HasValue && matchup.OverallWinner.Value != 0)
        {
            var winner = matchup.OverallWinner.Value == 1 ? matchup.Player1 : matchup.Player2;
            var loser = matchup.OverallWinner.Value == 1 ? matchup.Player2 : matchup.Player1;

            embed.WithDescription($":trophy: {MessageTextFormat.Bold(winner.Name)} wins this matchup {MessageTextFormat.Bold(winner.Wins.ToString())} to {MessageTextFormat.Bold(loser.Wins.ToString())}!");
        }
        else
        {
            embed.WithDescription($":scales: Both players go even.");
        }

        var trackNames = new List<string>();
        var player1Times = new List<string>();
        var player2Times = new List<string>();

        foreach (var comparison in matchup.Comparisons)
        {
            trackNames.Add(MessageTextFormat.Bold(comparison.TrackName));

            if (comparison.Winner == 1)
            {
                player1Times.Add(MessageTextFormat.Bold(comparison.Player1TimeFormatted));
                player2Times.Add(MessageTextFormat.Strikethrough(comparison.Player2TimeFormatted));
            }
            else if (comparison.Winner == 2)
            {
                player1Times.Add(MessageTextFormat.Strikethrough(comparison.Player1TimeFormatted));
                player2Times.Add(MessageTextFormat.Bold(comparison.Player2TimeFormatted));
            }
            else
            {
                player1Times.Add(MessageTextFormat.Bold(comparison.Player1TimeFormatted));
                player2Times.Add(MessageTextFormat.Bold(comparison.Player2TimeFormatted));
            }
        }

        var tracksField = new EmbedFieldBuilder()
            .WithName($":motorway: {MessageTextFormat.Bold("Track")}")
            .WithValue(string.Join("\n", trackNames))
            .WithIsInline(true);

        var player1TimesField = new EmbedFieldBuilder()
            .WithName($"{CountryEmoteMapper.MapCountryToEmote(matchup.Player1.CountryName)} {MessageTextFormat.Bold(matchup.Player1.Name)}")
            .WithValue(string.Join("\n", player1Times))
            .WithIsInline(true);

        var player2TimesField = new EmbedFieldBuilder()
            .WithName($"{CountryEmoteMapper.MapCountryToEmote(matchup.Player2.CountryName)} {MessageTextFormat.Bold(matchup.Player2.Name)}")
            .WithValue(string.Join("\n", player2Times))
            .WithIsInline(true);

        embed.AddField(tracksField);
        embed.AddField(player1TimesField);
        embed.AddField(player2TimesField);

        return embed;
    }
}
