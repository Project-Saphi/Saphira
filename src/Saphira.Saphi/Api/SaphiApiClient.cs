using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Core;
using Saphira.Core.Extensions.Caching;
using Saphira.Core.Logging;
using Saphira.Saphi.Api.Response;
using System.Diagnostics;
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
    private readonly IMessageLogger _logger;

    public SaphiApiClient(HttpClient httpClient, Configuration configuration, IMemoryCache cache, CacheInvalidationService cacheInvalidationService, IMessageLogger logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _cache = cache;
        _cacheInvalidationService = cacheInvalidationService;
        _logger = logger;

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

    public async Task<SaphiApiResult<GetCustomTrackResponse>> GetCustomTrackAsync(int trackId, TimeSpan? cacheDuration = null, bool forceRefresh = false)
    {
        var endpoint = $"{SaphiApiEndpoint.Tracks}/{trackId}";
        var cacheKey = $"api:custom_track:{trackId}";

        return forceRefresh
            ? await GetAsync<GetCustomTrackResponse>(endpoint)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<GetCustomTrackResponse>(endpoint),
                cacheDuration ?? DefaultCacheDuration.CustomTrack
            );
    }

    public async Task<SaphiApiResult<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCustomTracksResponse>(SaphiApiEndpoint.Tracks)
            : await GetCachedAsync(
                "api:custom_tracks",
                () => GetAsync<GetCustomTracksResponse>(SaphiApiEndpoint.Tracks),
                cacheDuration ?? DefaultCacheDuration.CustomTrack
            );

    public async Task<SaphiApiResult<GetPlayerPBsResponse>> GetPlayerPBsAsync(
        int playerId,
        int? trackId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var endpoint = $"{SaphiApiEndpoint.Players}/{playerId}/pbs";
        var queryParams = new Dictionary<string, object>();

        if (trackId.HasValue)
            queryParams["track_id"] = trackId.Value;

        var cacheKey = $"api:player_pbs:{playerId}:{trackId?.ToString() ?? "all"}";

        return forceRefresh
            ? await GetAsync<GetPlayerPBsResponse>(endpoint, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<GetPlayerPBsResponse>(endpoint, queryParams),
                cacheDuration ?? DefaultCacheDuration.PlayerPB
            );
    }

    public async Task<SaphiApiResult<GetPlayerPBsResponse>> GetPlayerPBsByUsernameAsync(
        string username,
        int? trackId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var endpoint = $"{SaphiApiEndpoint.Players}/pbs";
        var queryParams = new Dictionary<string, object> { { "username", username } };

        if (trackId.HasValue)
            queryParams["track_id"] = trackId.Value;

        var cacheKey = $"api:player_pbs:username:{username}:{trackId?.ToString() ?? "all"}";

        return forceRefresh
            ? await GetAsync<GetPlayerPBsResponse>(endpoint, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<GetPlayerPBsResponse>(endpoint, queryParams),
                cacheDuration ?? DefaultCacheDuration.PlayerPB
            );
    }

    public async Task<SaphiApiResult<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(
        int trackId,
        int categoryId,
        int? pageIndex = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var endpoint = $"{SaphiApiEndpoint.Leaderboards}/{trackId}";
        var queryParams = new Dictionary<string, object>
        {
            { "type", categoryId }
        };

        if (pageIndex.HasValue)
            queryParams["page_index"] = pageIndex.Value;

        var cacheKey = $"api:track_leaderboard:{trackId}:{categoryId}:{pageIndex?.ToString() ?? "0"}";

        return forceRefresh
            ? await GetAsync<GetTrackLeaderboardResponse>(endpoint, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<GetTrackLeaderboardResponse>(endpoint, queryParams),
                cacheDuration ?? DefaultCacheDuration.Leaderboard
            );
    }

    public async Task<SaphiApiResult<GetRecentSubmissionsResponse>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        int? trackId = null,
        int? categoryId = null,
        int? userId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var endpoint = $"{SaphiApiEndpoint.Submissions}/recent";
        var queryParams = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(timeFilter))
            queryParams["time_filter"] = timeFilter;

        if (trackId.HasValue)
            queryParams["track_id"] = trackId.Value;

        if (categoryId.HasValue)
            queryParams["category_id"] = categoryId.Value;

        if (userId.HasValue)
            queryParams["user_id"] = userId.Value;

        var cacheKey = $"api:recent_submissions:{timeFilter ?? "24h"}:{trackId?.ToString() ?? "all"}:{categoryId?.ToString() ?? "all"}:{userId?.ToString() ?? "all"}";

        return forceRefresh
            ? await GetAsync<GetRecentSubmissionsResponse>(endpoint, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<GetRecentSubmissionsResponse>(endpoint, queryParams),
                cacheDuration ?? DefaultCacheDuration.RecentSubmission
            );
    }

    public async Task<SaphiApiResult<GetUserProfileResponse>> GetUserProfileAsync(
        int userId,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var endpoint = $"{SaphiApiEndpoint.Users}/{userId}/profile";

        return forceRefresh
            ? await GetAsync<GetUserProfileResponse>(endpoint)
            : await GetCachedAsync(
                $"api:user_profile:{userId}",
                () => GetAsync<GetUserProfileResponse>(endpoint),
                cacheDuration ?? DefaultCacheDuration.Player
            );
    }

    public async Task<SaphiApiResult<GetUserProfileResponse>> GetUserProfileByUsernameAsync(
        string username,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var endpoint = $"{SaphiApiEndpoint.Users}/profile";
        var queryParams = new Dictionary<string, object> { { "username", username } };

        return forceRefresh
            ? await GetAsync<GetUserProfileResponse>(endpoint, queryParams)
            : await GetCachedAsync(
                $"api:user_profile:username:{username}",
                () => GetAsync<GetUserProfileResponse>(endpoint, queryParams),
                cacheDuration ?? DefaultCacheDuration.Player
            );
    }

    public async Task<SaphiApiResult<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCountriesResponse>(SaphiApiEndpoint.Countries)
            : await GetCachedAsync(
                "api:countries",
                () => GetAsync<GetCountriesResponse>(SaphiApiEndpoint.Countries),
                cacheDuration ?? DefaultCacheDuration.Country
            );

    public async Task<SaphiApiResult<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCharactersResponse>(SaphiApiEndpoint.Characters)
            : await GetCachedAsync(
                "api:characters",
                () => GetAsync<GetCharactersResponse>(SaphiApiEndpoint.Characters),
                cacheDuration ?? DefaultCacheDuration.Character
            );

    public async Task<SaphiApiResult<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetEngineTypesResponse>(SaphiApiEndpoint.EngineTypes)
            : await GetCachedAsync(
                "api:engine_types",
                () => GetAsync<GetEngineTypesResponse>(SaphiApiEndpoint.EngineTypes),
                cacheDuration ?? DefaultCacheDuration.Engine
            );

    public async Task<SaphiApiResult<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<GetCategoriesResponse>(SaphiApiEndpoint.Types)
            : await GetCachedAsync(
                "api:categories",
                () => GetAsync<GetCategoriesResponse>(SaphiApiEndpoint.Types),
                cacheDuration ?? DefaultCacheDuration.Category
            );

    public async Task<SaphiApiResult<GetPlayersResponse>> GetPlayersAsync(
        string? search = null,
        int? countryId = null,
        int? status = null,
        string? discord = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(search))
            queryParams["search"] = search;

        if (countryId.HasValue)
            queryParams["country"] = countryId.Value;

        if (status.HasValue)
            queryParams["status"] = status.Value;

        if (!string.IsNullOrWhiteSpace(discord))
            queryParams["discord"] = discord;

        var cacheKey = $"api:players:{search ?? "all"}:{countryId?.ToString() ?? "all"}:{status?.ToString() ?? "all"}:{discord ?? "all"}";

        return forceRefresh
            ? await GetAsync<GetPlayersResponse>(SaphiApiEndpoint.Players, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<GetPlayersResponse>(SaphiApiEndpoint.Players, queryParams),
                cacheDuration ?? DefaultCacheDuration.Players
            );
    }

    private string BuildUrl(string endpoint, Dictionary<string, object>? queryParams = null)
    {
        if (queryParams == null || queryParams.Count == 0)
            return endpoint;

        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in queryParams)
        {
            query[param.Key] = param.Value.ToString();
        }

        return $"{endpoint}?{query}";
    }

    private async Task<SaphiApiResult<T>> GetAsync<T>(string endpoint)
    {
        var stopWatch = new Stopwatch();

        try
        {
            stopWatch.Start();
            _logger.Log(LogSeverity.Debug, "Saphira", $"Calling Saphi API endpoint {endpoint} ...");

            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            stopWatch.Stop();
            _logger.Log(LogSeverity.Debug, "Saphira", $"Response received from Saphi API endpoint {endpoint} in {stopWatch.ElapsedMilliseconds}ms");

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

    private async Task<SaphiApiResult<T>> GetAsync<T>(string endpoint, Dictionary<string, object> queryParams)
    {
        var url = BuildUrl(endpoint, queryParams);
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
