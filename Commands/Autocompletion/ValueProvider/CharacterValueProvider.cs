using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CharacterValueProvider(CachedClient client, IMessageLogger logger, IMemoryCache cache) : IValueProvider
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
}
