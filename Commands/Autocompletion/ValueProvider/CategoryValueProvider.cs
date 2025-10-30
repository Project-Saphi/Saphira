using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CategoryValueProvider : IValueProvider
    {
        private readonly CachedClient _client;
        private readonly IMessageLogger _logger;
        private readonly IMemoryCache _cache;

        public CategoryValueProvider(CachedClient client, IMessageLogger logger, IMemoryCache cache)
        {
            _client = client;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var result = await _client.GetCategoriesAsync();

            if (result.Success == false || result.Response == null)
            {
                _logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch categories: {result.ErrorMessage ?? "Unknown error"}");
                return values;
            }

            foreach (var category in result.Response.Data)
            {
                var value = new Value(int.Parse(category.Id), category.Name);
                values.Add(value);
            }

            return values;
        }
    }
}
