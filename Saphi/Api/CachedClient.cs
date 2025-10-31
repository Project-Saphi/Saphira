using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Extensions.Caching;
using Saphira.Saphi.Api.Response;

namespace Saphira.Saphi.Api
{
    public class CachedClient
    {
        private readonly HttpClient _httpClient;
        private readonly Configuration _configuration;
        private readonly IMemoryCache _cache;
        private readonly CacheInvalidationService _cacheInvalidationService;

        public CachedClient(HttpClient httpClient, Configuration configuration, IMemoryCache cache, CacheInvalidationService cacheInvalidationService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _cacheInvalidationService = cacheInvalidationService;

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
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
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

        public async Task<Result<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null)
        {
            return await _cache.GetOrCreateAsync("api:custom_tracks", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.CustomTrack;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                return await GetAsync<GetCustomTracksResponse>(Endpoint.GetCustomTracks);
            }) ?? new Result<GetCustomTracksResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }

        public async Task<Result<GetPlayerPBsResponse>> GetPlayerPBsAsync(string playerId, TimeSpan? cacheDuration = null)
        {
            var cacheKey = $"api:player_pbs:{playerId}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.PlayerPB;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                var queryParams = new Dictionary<string, string>
                {
                    { "player_id", playerId }
                };

                return await GetAsync<GetPlayerPBsResponse>(Endpoint.GetPlayerPBs, queryParams);
            }) ?? new Result<GetPlayerPBsResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }

        public async Task<Result<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(string trackId, string categoryId, TimeSpan? cacheDuration = null)
        {
            var cacheKey = $"api:track_leaderboard:{trackId}:{categoryId}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.Leaderboard;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                var queryParams = new Dictionary<string, string>
                {
                    { "id", trackId },
                    { "type", categoryId }
                };

                return await GetAsync<GetTrackLeaderboardResponse>(Endpoint.GetTrackLeaderboard, queryParams);
            }) ?? new Result<GetTrackLeaderboardResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }

        public async Task<Result<GetUserProfileResponse>> GetUserProfileAsync(string userId, TimeSpan? cacheDuration = null)
        {
            var cacheKey = $"api:user_profile:{userId}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.Player;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                var queryParams = new Dictionary<string, string>
                {
                    { "user_id", userId }
                };

                return await GetAsync<GetUserProfileResponse>(Endpoint.GetUserProfile, queryParams);
            }) ?? new Result<GetUserProfileResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }

        public async Task<Result<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null)
        {
            return await _cache.GetOrCreateAsync("api:countries", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.Country;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                return await GetAsync<GetCountriesResponse>(Endpoint.GetCountries);
            }) ?? new Result<GetCountriesResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }

        public async Task<Result<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null)
        {
            return await _cache.GetOrCreateAsync("api:characters", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.Character;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                return await GetAsync<GetCharactersResponse>(Endpoint.GetCharacters);
            }) ?? new Result<GetCharactersResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }

        public async Task<Result<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null)
        {
            return await _cache.GetOrCreateAsync("api:engine_types", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.Engine;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                return await GetAsync<GetEngineTypesResponse>(Endpoint.GetEngineTypes);
            }) ?? new Result<GetEngineTypesResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }

        public async Task<Result<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null)
        {
            return await _cache.GetOrCreateAsync("api:categories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = cacheDuration ?? DefaultCacheDuration.Category;
                entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());

                return await GetAsync<GetCategoriesResponse>(Endpoint.GetCategories);
            }) ?? new Result<GetCategoriesResponse>
            {
                Success = false,
                ErrorMessage = "Cache returned null"
            };
        }
    }
}
