using Discord;
using Saphira.Discord.Extensions.DependencyInjection;
using Saphira.Discord.Logging;
using Saphira.Saphi.Api;

namespace Saphira.Discord.Interaction.Autocompletion.ValueProvider;

[AutoRegister]
public class CharacterValueProvider(CachedClient client, IMessageLogger logger) : IValueProvider
{
    public async Task<List<Value>> GetValuesAsync()
    {
        var result = await client.GetCharactersAsync();

        if (!result.Success || result.Response?.Data == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch characters: {result.ErrorMessage ?? "Unknown error"}");
            return [];
        }

        return [.. result.Response.Data.Select(c => new Value(int.Parse(c.Id), c.Name))];
    }
}
