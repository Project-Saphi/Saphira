using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CategoryValueProvider : IValueProvider
    {
        private readonly SaphiApiClient _saphiApiClient;
        private readonly IMemoryCache _cache;

        public CategoryValueProvider(SaphiApiClient saphiApiClient, IMemoryCache cache)
        {
            _saphiApiClient = saphiApiClient;
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

                var response = await _saphiApiClient.GetCategoriesAsync();
                return response?.Data ?? [];
            });
        }
    }
}
