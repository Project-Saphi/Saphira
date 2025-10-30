using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CustomTrackValueProvider : IValueProvider
    {
        private readonly CachedClient _client;
        private readonly IMessageLogger _logger;
        private readonly IMemoryCache _cache;

        public CustomTrackValueProvider(CachedClient client, IMessageLogger logger, IMemoryCache cache)
        {
            _client = client;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var result = await _client.GetCustomTracksAsync();

            if (result.Success == false || result.Response == null)
            {
                _logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch custom tracks: {result.ErrorMessage ?? "Unknown error"}");
                return values;
            }

            foreach (var customTrack in result.Response.Data)
            {
                var value = new Value(int.Parse(customTrack.Id), customTrack.Name);
                values.Add(value);
            }

            return values;
        }
    }
}
