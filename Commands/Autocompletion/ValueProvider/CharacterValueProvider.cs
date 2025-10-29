using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CharacterValueProvider : IValueProvider
    {
        private readonly SaphiApiClient _saphiApiClient;
        private readonly IMemoryCache _cache;

        public CharacterValueProvider(SaphiApiClient saphiApiClient, IMemoryCache cache)
        {
            _saphiApiClient = saphiApiClient;        
            _cache = cache;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var characters = await GetCharactersFromCache();

            foreach (var character in characters)
            {
                var value = new Value(int.Parse(character.Id), character.Name);
                values.Add(value);
            }

            return values;
        }

        private async Task<List<Character>> GetCharactersFromCache()
        {
            return await _cache.GetOrCreateAsync("characters", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

                var response = await _saphiApiClient.GetCharactersAsync();
                return response?.Data ?? [];
            });
        }
    }
}
