using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Autocompletion;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Util.EmoteMapper;
using Saphira.Util.Game.Matchup;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class MatchupCommand(PlayerMatchupCalculator playerMatchupGenerator) : BaseCommand
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
        [Autocomplete(typeof(PlayerAutocompleteHandler))] string player1,
        [Autocomplete(typeof(PlayerAutocompleteHandler))] string player2,
        [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
        )
    {
        var result = await playerMatchupGenerator.GeneratePlayerMatchup(player1, player2, category);

        if (result.Status == PlayerMatchupCalculationStatus.Failure)
        {
            var errorAlert = new ErrorAlertEmbedBuilder(result.ErrorMessage ?? "An error occured calculating the matchup.");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        var matchupEmbed = BuildMatchupEmbed(result.PlayerMatchup);
        await RespondAsync(embed: matchupEmbed.Build());
    }

    private EmbedBuilder BuildMatchupEmbed(PlayerMatchup playerMatchup)
    {
        var embed = new EmbedBuilder()
            .WithAuthor($"Matchup between {playerMatchup.PlayerName1} and {playerMatchup.PlayerName2} ({playerMatchup.Category.Name})");

        if (!playerMatchup.IsEvenMatchup())
        {
            embed.WithDescription($":trophy: {MessageTextFormat.Bold(playerMatchup.WinnerName)} wins this matchup {MessageTextFormat.Bold(playerMatchup.WinnerWins.ToString())} to {MessageTextFormat.Bold(playerMatchup.LoserWins.ToString())}!");
        }
        else
        {
            embed.WithDescription($":scales: Both players go even.");
        }

        var trackNames = new List<string>();
        var player1Times = new List<string>();
        var player2Times = new List<string>();

        foreach (var playerRecordComparison in playerMatchup.PlayerRecordComparisons)
        {
            trackNames.Add(MessageTextFormat.Bold(playerRecordComparison.TrackName));

            if (!playerRecordComparison.IsEvenRecord())
            {
                if (playerRecordComparison.WinnerName == playerMatchup.PlayerName1)
                {
                    player1Times.Add(MessageTextFormat.Bold(playerRecordComparison.WinnerTime));
                    player2Times.Add(MessageTextFormat.Strikethrough(playerRecordComparison.LoserTime));
                }
                else
                {
                    player2Times.Add(MessageTextFormat.Bold(playerRecordComparison.WinnerTime));
                    player1Times.Add(MessageTextFormat.Strikethrough(playerRecordComparison.LoserTime));
                }
            }
            else
            {
                player1Times.Add(MessageTextFormat.Bold(playerRecordComparison.WinnerTime));
                player2Times.Add(MessageTextFormat.Bold(playerRecordComparison.WinnerTime));
            }
        }

        var tracksField = new EmbedFieldBuilder()
            .WithName($":motorway: {MessageTextFormat.Bold("Track")}")
            .WithValue(string.Join("\n", trackNames))
            .WithIsInline(true);

        var winnerTimesField = new EmbedFieldBuilder()
            .WithName($"{CountryEmoteMapper.MapCountryToEmote(playerMatchup.CountryName1)} {MessageTextFormat.Bold(playerMatchup.PlayerName1)}")
            .WithValue(string.Join("\n", player1Times))
            .WithIsInline(true);

        var loserTimesField = new EmbedFieldBuilder()
            .WithName($"{CountryEmoteMapper.MapCountryToEmote(playerMatchup.CountryName2)} {MessageTextFormat.Bold(playerMatchup.PlayerName2)}")
            .WithValue(string.Join ("\n", player2Times))
            .WithIsInline(true);

        embed.AddField(tracksField);
        embed.AddField(winnerTimesField);
        embed.AddField(loserTimesField);

        return embed;
    }
}
