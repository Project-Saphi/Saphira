using Discord;
using Discord.Commands;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Util.Game.Matchup;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class MatchupCommand(PlayerMatchupGenerator playerMatchupGenerator) : InteractionModuleBase<SocketInteractionContext>
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("matchup", "Show the matchup between 2 players")]
    public async Task HandleCommand(
        [Autocomplete(typeof(PlayerAutocompleteHandler))] string player1,
        [Autocomplete(typeof(PlayerAutocompleteHandler))] string player2,
        [Autocomplete(typeof(CategoryAutocompleteHandler))] string category
        )
    {
        var result = await playerMatchupGenerator.GeneratePlayerMatchup(player1, player2, category);

        if (result.StatusCode == PlayerMatchupGeneratorResult.Status.Failure)
        {
            var errorAlert = new ErrorAlertEmbedBuilder(result.ErrorMessage ?? "test");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        await DeferAsync();
    }
}
