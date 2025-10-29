using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace Saphira.Saphi.Api
{
    public class SaphiApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly Configuration _configuration;

        public SaphiApiClient(HttpClient httpClient, Configuration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            if (!string.IsNullOrWhiteSpace(_configuration.SaphiApiBaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_configuration.SaphiApiBaseUrl);
            }

            if (!string.IsNullOrWhiteSpace(_configuration.SaphiApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Saphi-Api-Key", _configuration.SaphiApiKey);
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private string BuildUrlWithQuery(string endpoint, Dictionary<string, string>? queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
                return endpoint;

            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }

            return $"{endpoint}?{query}";
        }

        private async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
        {
            var url = BuildUrlWithQuery(endpoint, queryParams);
            return await GetAsync<T>(url);
        }

        public async Task<GetCustomTracksResponse> GetCustomTracksAsync()
        {
            return await GetAsync<GetCustomTracksResponse>(SaphiApiEndpoint.GetCustomTracks);
        }

        public async Task<GetPlayerPBsResponse> GetPlayerPBsAsync(string playerId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "player_id", playerId }
            };

            return await GetAsync<GetPlayerPBsResponse>(SaphiApiEndpoint.GetPlayerPBs, queryParams);
        }

        public async Task<GetTrackLeaderboardResponse> GetTrackLeaderboardAsync(string trackId, string categoryId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "id", trackId },
                { "type", categoryId }
            };

            return await GetAsync<GetTrackLeaderboardResponse>(SaphiApiEndpoint.GetTrackLeaderboard, queryParams);
        }

        public async Task<GetUserProfileResponse> GetUserProfileAsync(string userId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "user_id", userId }
            };

            return await GetAsync<GetUserProfileResponse>(SaphiApiEndpoint.GetUserProfile, queryParams);
        }

        public async Task<GetCountriesResponse> GetCountriesAsync()
        {
            return await GetAsync<GetCountriesResponse>(SaphiApiEndpoint.GetCountries);
        }

        public async Task<GetCharactersResponse> GetCharactersAsync()
        {
            return await GetAsync<GetCharactersResponse>(SaphiApiEndpoint.GetCharacters);
        }

        public async Task<GetEngineTypesResponse> GetEngineTypesAsync()
        {
            return await GetAsync<GetEngineTypesResponse>(SaphiApiEndpoint.GetEngineTypes);
        }

        public async Task<GetCategoriesResponse> GetCategoriesAsync()
        {
            return await GetAsync<GetCategoriesResponse>(SaphiApiEndpoint.GetCategories);
        }
    }
}
