using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Core;
using Saphira.Discord.Interaction.Foundation.Autocompletion.ValueProvider;

namespace Saphira.Discord.Interaction.Foundation.Autocompletion;

public class GenericAutocompleteHandler<TProvider> : AutocompleteHandler
    where TProvider : IValueProvider
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services
        )
    {
        try
        {
            var configuration = services.GetRequiredService<Configuration>();
            var provider = services.GetRequiredService<TProvider>();
            var values = await provider.GetValuesAsync();

            if (values != null && values.Count > 0)
            {
                var userInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";
                var suggestionCount = GetMaxSuggestionCount(configuration);

                var suggestions = values
                    .Where(v => v.Name.Contains(userInput, StringComparison.OrdinalIgnoreCase) || v.Id.ToString().Contains(userInput))
                    .Take(suggestionCount)
                    .Select(v => new AutocompleteResult(v.Name, v.Id.ToString()));

                return AutocompletionResult.FromSuccess(suggestions);
            }

            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "No values found");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {typeof(TProvider).Name} autocompletion: {ex.Message}");
            return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "An error occurred");
        }
    }

    private int GetMaxSuggestionCount(Configuration botConfiguration)
    {
        var suggestionsCount = botConfiguration.MaxAutocompleteSuggestions;

        // Discord is limited to 25 suggestions at most
        // We default to 15 if the owner configures some bullshit
        if (suggestionsCount < 1 || suggestionsCount > 25)
        {
            suggestionsCount = 15;
        }

        return suggestionsCount;
    }
}
