using Microsoft.Extensions.Caching.Memory;
using Saphira.Core;
using Saphira.Core.Extensions.Caching;
using Saphira.Saphi.Api.Response;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Saphira.Saphi.Api;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly Configuration _configuration;
    private readonly IMemoryCache _cache;
    private readonly CacheInvalidationService _cacheInvalidationService;

    public ApiClient(HttpClient httpClient, Configuration configuration, IMemoryCache cache, CacheInvalidationService cacheInvalidationService)
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

        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<ApiResult<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            "api:custom_tracks",
            () => GetAsync<GetCustomTracksResponse>(ApiEndpoint.GetCustomTracks),
            cacheDuration ?? DefaultCacheDuration.CustomTrack
        );

    public async Task<ApiResult<GetPlayerPBsResponse>> GetPlayerPBsAsync(string playerId, TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            $"api:player_pbs:{playerId}",
            () => GetAsync<GetPlayerPBsResponse>(ApiEndpoint.GetPlayerPBs, new Dictionary<string, string>
            {
                { "player_id", playerId }
            }),
            cacheDuration ?? DefaultCacheDuration.PlayerPB
        );

    public async Task<ApiResult<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(string trackId, string categoryId, TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            $"api:track_leaderboard:{trackId}:{categoryId}",
            () => GetAsync<GetTrackLeaderboardResponse>(ApiEndpoint.GetTrackLeaderboard, new Dictionary<string, string>
            {
                { "id", trackId },
                { "type", categoryId }
            }),
            cacheDuration ?? DefaultCacheDuration.Leaderboard
        );

    public async Task<ApiResult<GetRecentSubmissionsResponse>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        string? trackId = null,
        string? categoryId = null,
        string? userId = null,
        TimeSpan? cacheDuration = null)
    {
        var queryParams = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(timeFilter))
            queryParams["time_filter"] = timeFilter;

        if (!string.IsNullOrWhiteSpace(trackId))
            queryParams["track_id"] = trackId;

        if (!string.IsNullOrWhiteSpace(categoryId))
            queryParams["category_id"] = categoryId;

        if (!string.IsNullOrWhiteSpace(userId))
            queryParams["user_id"] = userId;

        var cacheKey = $"api:recent_submissions:{timeFilter ?? "24h"}:{trackId ?? "all"}:{categoryId ?? "all"}:{userId ?? "all"}";

        return await GetCachedAsync(
            cacheKey,
            () => GetAsync<GetRecentSubmissionsResponse>(ApiEndpoint.GetRecentSubmissions, queryParams),
            cacheDuration ?? DefaultCacheDuration.RecentSubmission
        );
    }

    public async Task<ApiResult<GetUserProfileResponse>> GetUserProfileAsync(string userId, TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            $"api:user_profile:{userId}",
            () => GetAsync<GetUserProfileResponse>(ApiEndpoint.GetUserProfile, new Dictionary<string, string>
            {
                { "user_id", userId }
            }),
            cacheDuration ?? DefaultCacheDuration.Player
        );

    public async Task<ApiResult<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            "api:countries",
            () => GetAsync<GetCountriesResponse>(ApiEndpoint.GetCountries),
            cacheDuration ?? DefaultCacheDuration.Country
        );

    public async Task<ApiResult<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            "api:characters",
            () => GetAsync<GetCharactersResponse>(ApiEndpoint.GetCharacters),
            cacheDuration ?? DefaultCacheDuration.Character
        );

    public async Task<ApiResult<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            "api:engine_types",
            () => GetAsync<GetEngineTypesResponse>(ApiEndpoint.GetEngineTypes),
            cacheDuration ?? DefaultCacheDuration.Engine
        );

    public async Task<ApiResult<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null) =>
        await GetCachedAsync(
            "api:categories",
            () => GetAsync<GetCategoriesResponse>(ApiEndpoint.GetCategories),
            cacheDuration ?? DefaultCacheDuration.Category
        );

    public async Task<ApiResult<GetPlayersResponse>> GetPlayersAsync(TimeSpan? cacheDuration = null) => 
        await  GetCachedAsync(
            "api:players",
            () => GetAsync<GetPlayersResponse>(ApiEndpoint.GetPlayers),
            cacheDuration ?? DefaultCacheDuration.Players
        );

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

    private async Task<ApiResult<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<T>
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

            return new ApiResult<T>
            {
                Success = true,
                Response = data,
                StatusCode = response.StatusCode
            };
        }
        catch (JsonException ex)
        {
            return new ApiResult<T>
            {
                Success = false,
                ErrorMessage = $"Failed to parse JSON: {ex.Message}"
            };
        }
        catch (TaskCanceledException ex)
        {
            return new ApiResult<T>
            {
                Success = false,
                ErrorMessage = $"A timeout occured when calling the API: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResult<T>
            {
                Success = false,
                ErrorMessage = $"Request failed: {ex.Message}"
            };
        }
    }

    private async Task<ApiResult<T>> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
    {
        var url = BuildUrlWithQuery(endpoint, queryParams);
        return await GetAsync<T>(url);
    }

    private async Task<ApiResult<T>> GetCachedAsync<T>(
        string cacheKey,
        Func<Task<ApiResult<T>>> fetchFunc,
        TimeSpan cacheDuration) where T : class
    {
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = cacheDuration;
            entry.AddExpirationToken(_cacheInvalidationService.GetInvalidationToken());
            
            var result = await fetchFunc();

            // it is possible that the request fails and the result is not successfull,
            // so we set the cache duration to 0 to avoid cache poisoning
            if (!result.Success)
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMicroseconds(1);
            }

            return result;
        }) ?? new ApiResult<T>
        {
            Success = false,
            ErrorMessage = "Cache returned null"
        };
    }
}
