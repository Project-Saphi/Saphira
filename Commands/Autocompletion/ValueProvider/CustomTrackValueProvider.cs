using Microsoft.Extensions.Caching.Memory;
using Saphira.Saphi.Api;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CustomTrackValueProvider : IValueProvider
    {
        private readonly SaphiApiClient _saphiApiClient;
        private readonly IMemoryCache _cache;

        public CustomTrackValueProvider(SaphiApiClient saphiApiClient, IMemoryCache cache)
        {
            _saphiApiClient = saphiApiClient;        
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

                var response = await _saphiApiClient.GetCustomTracksAsync();
                return response?.Data ?? [];
            });
        }
    }
}
