using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CategoryValueProvider : IValueProvider
    {
        private readonly Client _client;
        private readonly IMessageLogger _logger;
        private readonly IMemoryCache _cache;

        public CategoryValueProvider(Client client, IMessageLogger logger, IMemoryCache cache)
        {
            _client = client;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var categories = await GetCategoriesFromCache();

            foreach (var category in categories)
            {
                var value = new Value(int.Parse(category.Id), category.Name);
                values.Add(value);
            }

            return values;
        }

        private async Task<List<Category>> GetCategoriesFromCache()
        {
            return await _cache.GetOrCreateAsync("categories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

                var result = await _client.GetCategoriesAsync();

                if (result.Success == true)
                {
                    var response = result.Response;
                    return response?.Data ?? [];
                }

                if (result.ErrorMessage != null)
                {
                    _logger.Log(LogSeverity.Error, "Saphira", result.ErrorMessage);
                }

                return [];
            }) ?? [];
        }
    }
}
