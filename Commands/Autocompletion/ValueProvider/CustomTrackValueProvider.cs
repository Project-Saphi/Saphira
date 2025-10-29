using Saphira.Saphi.Api;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class CustomTrackValueProvider : IValueProvider
    {
        private readonly SaphiApiClient _saphiApiClient;

        public CustomTrackValueProvider(SaphiApiClient saphiApiClient)
        {
            _saphiApiClient = saphiApiClient;        
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            var values = new List<Value>();
            var response = await _saphiApiClient.GetCustomTracksAsync();

            if (response?.Success == true)
            {
                foreach (var customTrack in response.Data)
                {
                    var value = new Value(int.Parse(customTrack.Id), customTrack.Name);
                    values.Add(value);
                }
            }

            return values;
        }
    }
}
