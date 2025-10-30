using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CharacterValueProvider : IValueProvider
    {
        private readonly CachedClient _client;
        private readonly IMessageLogger _logger;
        private readonly IMemoryCache _cache;

        public CharacterValueProvider(CachedClient client, IMessageLogger logger, IMemoryCache cache)
        {
            _client = client;        
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var result = await _client.GetCharactersAsync();

            if (!result.Success || result.Response == null)
            {
                _logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch characters: {result.ErrorMessage ?? "Unknown error"}");
                return values;
            }

            foreach (var character in result.Response.Data)
            {
                var value = new Value(int.Parse(character.Id), character.Name);
                values.Add(value);
            }

            return values;
        }
    }
}
