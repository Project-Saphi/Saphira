using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CharacterValueProvider : IValueProvider
    {
        private readonly Client _client;
        private readonly IMessageLogger _logger;
        private readonly IMemoryCache _cache;

        public CharacterValueProvider(Client client, IMessageLogger logger, IMemoryCache cache)
        {
            _client = client;        
            _logger = logger;
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

                var result = await _client.GetCharactersAsync();

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
