using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CustomTrackValueProvider : IValueProvider
    {
        private readonly Client _client;
        private readonly IMessageLogger _logger;
        private readonly IMemoryCache _cache;

        public CustomTrackValueProvider(Client client, IMessageLogger logger, IMemoryCache cache)
        {
            _client = client;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var customTracks = await GetCustomTracksFromCache();

            foreach (var customTrack in customTracks)
            {
                var value = new Value(int.Parse(customTrack.Id), customTrack.Name);
                values.Add(value);
            }

            return values;
        }

        private async Task<List<CustomTrack>> GetCustomTracksFromCache()
        {
            return await _cache.GetOrCreateAsync("custom_tracks", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

                var result = await _client.GetCustomTracksAsync();

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
