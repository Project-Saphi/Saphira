using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Saphira.Commands.Autocompletion.ValueProvider;

namespace Saphira.Commands.Autocompletion
{
    public abstract class BasicAutocompleteHandler<TProvider> : AutocompleteHandler
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
                var provider = services.GetRequiredService<TProvider>();
                var values = await provider.GetValuesAsync();

                if (values != null && values.Count > 0)
                {
                    var userInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

                    // Discord is limited to 25 suggestions at most, but I think 10 is sufficient
                    // Might increase it to 25 if people complain that 10 is too little ...
                    var suggestions = values
                        .Where(v => v.Name.Contains(userInput, StringComparison.OrdinalIgnoreCase) || v.Id.ToString().Contains(userInput))
                        .Take(10)
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
    }

    public class CustomTrackAutocompleteHandler : BasicAutocompleteHandler<CustomTrackValueProvider> { }
    public class CategoryAutocompleteHandler : BasicAutocompleteHandler<CategoryValueProvider> { }
    public class CharacterAutocompleteHandler : BasicAutocompleteHandler<CharacterValueProvider> { }
    public class ToggleableRoleAutocompleteHandler : BasicAutocompleteHandler<ToggleableRoleValueProvider> { }
}
