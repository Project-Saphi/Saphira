using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CategoryValueProvider(CachedClient client, IMessageLogger logger, IMemoryCache cache) : IValueProvider
    {
        public async Task<List<Value>> GetValuesAsync()
        {
            var result = await client.GetCategoriesAsync();

            if (!result.Success || result.Response == null)
            {
                logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch categories: {result.ErrorMessage ?? "Unknown error"}");
                return [];
            }

            return [.. result.Response.Data.Select(c => new Value(int.Parse(c.Id), c.Name))];
        }
    }
}
