using Microsoft.Extensions.Caching.Memory;
using Saphira.Core;
using Saphira.Core.Extensions.Caching;
using Saphira.Saphi.Api.Response;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Saphira.Saphi.Api;

public class SaphiApiClient : ISaphiApiClient
{
    private readonly HttpClient _httpClient;
    private readonly Configuration _configuration;
    private readonly IMemoryCache _cache;
    private readonly CacheInvalidationService _cacheInvalidationService;

    public SaphiApiClient(HttpClient httpClient, Configuration configuration, IMemoryCache cache, CacheInvalidationService cacheInvalidationService)
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

    public async Task<SaphiApiResult<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCustomTracksResponse>(SaphiApiEndpoint.GetCustomTracks)
            : await GetCachedAsync(
                "api:custom_tracks",
                () => GetAsync<GetCustomTracksResponse>(SaphiApiEndpoint.GetCustomTracks),
                cacheDuration ?? DefaultCacheDuration.CustomTrack
            );

    public async Task<SaphiApiResult<GetPlayerPBsResponse>> GetPlayerPBsAsync(string playerId, TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetPlayerPBsResponse>(SaphiApiEndpoint.GetPlayerPBs, new Dictionary<string, string>
            {
                { "player_id", playerId }
            })
            : await GetCachedAsync(
                $"api:player_pbs:{playerId}",
                () => GetAsync<GetPlayerPBsResponse>(SaphiApiEndpoint.GetPlayerPBs, new Dictionary<string, string>
                {
                    { "player_id", playerId }
                }),
                cacheDuration ?? DefaultCacheDuration.PlayerPB
            );

    public async Task<SaphiApiResult<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(string trackId, string categoryId, TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetTrackLeaderboardResponse>(SaphiApiEndpoint.GetTrackLeaderboard, new Dictionary<string, string>
            {
                { "id", trackId },
                { "type", categoryId }
            })
            : await GetCachedAsync(
                $"api:track_leaderboard:{trackId}:{categoryId}",
                () => GetAsync<GetTrackLeaderboardResponse>(SaphiApiEndpoint.GetTrackLeaderboard, new Dictionary<string, string>
                {
                    { "id", trackId },
                    { "type", categoryId }
                }),
                cacheDuration ?? DefaultCacheDuration.Leaderboard
            );

    public async Task<SaphiApiResult<GetRecentSubmissionsResponse>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        string? trackId = null,
        string? categoryId = null,
        string? userId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
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

        return forceRefresh
            ? await GetAsync<GetRecentSubmissionsResponse>(SaphiApiEndpoint.GetRecentSubmissions, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<GetRecentSubmissionsResponse>(SaphiApiEndpoint.GetRecentSubmissions, queryParams),
                cacheDuration ?? DefaultCacheDuration.RecentSubmission
            );
    }

    public async Task<SaphiApiResult<GetUserProfileResponse>> GetUserProfileAsync(string userId, TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetUserProfileResponse>(SaphiApiEndpoint.GetUserProfile, new Dictionary<string, string>
            {
                { "user_id", userId }
            })
            : await GetCachedAsync(
                $"api:user_profile:{userId}",
                () => GetAsync<GetUserProfileResponse>(SaphiApiEndpoint.GetUserProfile, new Dictionary<string, string>
                {
                    { "user_id", userId }
                }),
                cacheDuration ?? DefaultCacheDuration.Player
            );

    public async Task<SaphiApiResult<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCountriesResponse>(SaphiApiEndpoint.GetCountries)
            : await GetCachedAsync(
                "api:countries",
                () => GetAsync<GetCountriesResponse>(SaphiApiEndpoint.GetCountries),
                cacheDuration ?? DefaultCacheDuration.Country
            );

    public async Task<SaphiApiResult<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCharactersResponse>(SaphiApiEndpoint.GetCharacters)
            : await GetCachedAsync(
                "api:characters",
                () => GetAsync<GetCharactersResponse>(SaphiApiEndpoint.GetCharacters),
                cacheDuration ?? DefaultCacheDuration.Character
            );

    public async Task<SaphiApiResult<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetEngineTypesResponse>(SaphiApiEndpoint.GetEngineTypes)
            : await GetCachedAsync(
                "api:engine_types",
                () => GetAsync<GetEngineTypesResponse>(SaphiApiEndpoint.GetEngineTypes),
                cacheDuration ?? DefaultCacheDuration.Engine
            );

    public async Task<SaphiApiResult<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCategoriesResponse>(SaphiApiEndpoint.GetCategories)
            : await GetCachedAsync(
                "api:categories",
                () => GetAsync<GetCategoriesResponse>(SaphiApiEndpoint.GetCategories),
                cacheDuration ?? DefaultCacheDuration.Category
            );

    public async Task<SaphiApiResult<GetPlayersResponse>> GetPlayersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetPlayersResponse>(SaphiApiEndpoint.GetPlayers)
            : await GetCachedAsync(
                "api:players",
                () => GetAsync<GetPlayersResponse>(SaphiApiEndpoint.GetPlayers),
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

    private async Task<SaphiApiResult<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new SaphiApiResult<T>
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

            return new SaphiApiResult<T>
            {
                Success = true,
                Response = data,
                StatusCode = response.StatusCode
            };
        }
        catch (JsonException ex)
        {
            return new SaphiApiResult<T>
            {
                Success = false,
                ErrorMessage = $"Failed to parse JSON: {ex.Message}"
            };
        }
        catch (TaskCanceledException ex)
        {
            return new SaphiApiResult<T>
            {
                Success = false,
                ErrorMessage = $"A timeout occured when calling the API: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new SaphiApiResult<T>
            {
                Success = false,
                ErrorMessage = $"Request failed: {ex.Message}"
            };
        }
    }

    private async Task<SaphiApiResult<T>> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
    {
        var url = BuildUrlWithQuery(endpoint, queryParams);
        return await GetAsync<T>(url);
    }

    private async Task<SaphiApiResult<T>> GetCachedAsync<T>(
        string cacheKey,
        Func<Task<SaphiApiResult<T>>> fetchFunc,
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
        }) ?? new SaphiApiResult<T>
        {
            Success = false,
            ErrorMessage = "Cache returned null"
        };
    }
}
