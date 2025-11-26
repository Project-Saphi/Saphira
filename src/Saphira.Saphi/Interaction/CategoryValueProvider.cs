using Discord;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Discord.Interaction.Foundation.Autocompletion.ValueProvider;
using Saphira.Saphi.Api;

namespace Saphira.Saphi.Interaction;

[AutoRegister]
public class CategoryValueProvider(ISaphiApiClient client, IMessageLogger logger) : IValueProvider
{
    public async Task<List<Value>> GetValuesAsync()
    {
        var result = await client.GetCategoriesAsync();

        if (!result.Success || result.Response == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch categories: {result.ErrorMessage ?? "Unknown error"}");
            return [];
        }

        return [.. result.Response.Data.Select(c => new Value(c.Id, c.Name))];
    }
}
