using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace Saphira.Saphi.Api
{
    public class Client
    {
        private readonly HttpClient _httpClient;
        private readonly Configuration _configuration;

        public Client(HttpClient httpClient, Configuration configuration)
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

        private async Task<Result<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Result<T>
                    {
                        Success = false,
                        ErrorMessage = $"Saphi API returned HTTP status code {response.StatusCode}: {content}",
                        StatusCode = response.StatusCode
                    };
                }

                var data = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new Result<T>
                {
                    Success = true,
                    Response = data,
                    StatusCode = response.StatusCode
                };
            }
            catch (JsonException ex)
            {
                return new Result<T>
                {
                    Success = false,
                    ErrorMessage = $"Failed to parse JSON: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new Result<T>
                {
                    Success = false,
                    ErrorMessage = $"Request failed: {ex.Message}"
                };
            }
        }

        private async Task<Result<T>> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
        {
            var url = BuildUrlWithQuery(endpoint, queryParams);
            return await GetAsync<T>(url);
        }

        public async Task<Result<GetCustomTracksResponse>> GetCustomTracksAsync()
        {
            return await GetAsync<GetCustomTracksResponse>(Endpoint.GetCustomTracks);
        }

        public async Task<Result<GetPlayerPBsResponse>> GetPlayerPBsAsync(string playerId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "player_id", playerId }
            };

            return await GetAsync<GetPlayerPBsResponse>(Endpoint.GetPlayerPBs, queryParams);
        }

        public async Task<Result<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(string trackId, string categoryId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "id", trackId },
                { "type", categoryId }
            };

            return await GetAsync<GetTrackLeaderboardResponse>(Endpoint.GetTrackLeaderboard, queryParams);
        }

        public async Task<Result<GetUserProfileResponse>> GetUserProfileAsync(string userId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "user_id", userId }
            };

            return await GetAsync<GetUserProfileResponse>(Endpoint.GetUserProfile, queryParams);
        }

        public async Task<Result<GetCountriesResponse>> GetCountriesAsync()
        {
            return await GetAsync<GetCountriesResponse>(Endpoint.GetCountries);
        }

        public async Task<Result<GetCharactersResponse>> GetCharactersAsync()
        {
            return await GetAsync<GetCharactersResponse>(Endpoint.GetCharacters);
        }

        public async Task<Result<GetEngineTypesResponse>> GetEngineTypesAsync()
        {
            return await GetAsync<GetEngineTypesResponse>(Endpoint.GetEngineTypes);
        }

        public async Task<Result<GetCategoriesResponse>> GetCategoriesAsync()
        {
            return await GetAsync<GetCategoriesResponse>(Endpoint.GetCategories);
        }
    }
}
