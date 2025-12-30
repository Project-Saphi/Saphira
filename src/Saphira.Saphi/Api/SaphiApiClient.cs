using Discord;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Core;
using Saphira.Core.Extensions.Caching;
using Saphira.Core.Logging;
using Saphira.Saphi.Api.Response;
using Saphira.Saphi.Entity;
using Saphira.Saphi.Entity.Leaderboard;
using Saphira.Saphi.Entity.Ranking;
using Saphira.Saphi.Entity.Reference;
using Saphira.Saphi.Entity.User;
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

    public async Task<SaphiApiResult<SaphiApiResponse<CustomTrack>>> GetCustomTrackAsync(int trackId, TimeSpan? cacheDuration = null, bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object> { { "id", trackId } };
        var cacheKey = BuildCacheKey("api:custom_track", trackId);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<CustomTrack>>(SaphiApiEndpoint.Tracks, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<CustomTrack>>(SaphiApiEndpoint.Tracks, queryParams),
                cacheDuration ?? DefaultCacheDuration.CustomTrack
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<CustomTrack>>> GetCustomTracksAsync(
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:custom_tracks", page ?? 1, perPage ?? 100);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<CustomTrack>>(SaphiApiEndpoint.Tracks, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<CustomTrack>>(SaphiApiEndpoint.Tracks, queryParams),
                cacheDuration ?? DefaultCacheDuration.CustomTrack
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<PlayerPB>>> GetPlayerPBsAsync(
        int userId,
        int? trackId = null,
        int? categoryId = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object> { { "user_id", userId } };

        if (trackId.HasValue)
            queryParams["track_id"] = trackId.Value;

        if (categoryId.HasValue)
            queryParams["category_id"] = categoryId.Value;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:player_pbs", userId, trackId, categoryId, page ?? 1, perPage ?? 100);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<PlayerPB>>(SaphiApiEndpoint.Pbs, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<PlayerPB>>(SaphiApiEndpoint.Pbs, queryParams),
                cacheDuration ?? DefaultCacheDuration.PlayerPB
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<PlayerPB>>> GetPlayerPBsByUsernameAsync(
        string username,
        int? trackId = null,
        int? categoryId = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object> { { "username", username } };

        if (trackId.HasValue)
            queryParams["track_id"] = trackId.Value;

        if (categoryId.HasValue)
            queryParams["category_id"] = categoryId.Value;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:player_pbs:username", username, trackId, categoryId, page ?? 1, perPage ?? 100);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<PlayerPB>>(SaphiApiEndpoint.Pbs, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<PlayerPB>>(SaphiApiEndpoint.Pbs, queryParams),
                cacheDuration ?? DefaultCacheDuration.PlayerPB
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<TrackLeaderboardEntry>>> GetTrackLeaderboardAsync(
        int trackId,
        int categoryId,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>
        {
            { "track_id", trackId },
            { "category_id", categoryId }
        };

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:track_leaderboard", trackId, categoryId, page ?? 1, perPage ?? 100);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<TrackLeaderboardEntry>>(SaphiApiEndpoint.Leaderboards, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<TrackLeaderboardEntry>>(SaphiApiEndpoint.Leaderboards, queryParams),
                cacheDuration ?? DefaultCacheDuration.Leaderboard
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<RecentSubmission>>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        int? trackId = null,
        int? categoryId = null,
        int? userId = null,
        int? page = null,
        int? perPage = null,
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

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:recent_submissions", timeFilter ?? "24h", trackId, categoryId, userId, page ?? 1, perPage ?? 100);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<RecentSubmission>>(endpoint, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<RecentSubmission>>(endpoint, queryParams),
                cacheDuration ?? DefaultCacheDuration.RecentSubmission
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<UserProfile>>> GetUserProfileAsync(
        int userId,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object> { { "user_id", userId } };

        var cacheKey = BuildCacheKey("api:user_profile", userId);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<UserProfile>>(SaphiApiEndpoint.Profiles, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<UserProfile>>(SaphiApiEndpoint.Profiles, queryParams),
                cacheDuration ?? DefaultCacheDuration.UserProfile
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<UserProfile>>> GetUserProfileByUsernameAsync(
        string username,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object> { { "username", username } };
        var cacheKey = BuildCacheKey("api:user_profile:username", username);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<UserProfile>>(SaphiApiEndpoint.Profiles, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<UserProfile>>(SaphiApiEndpoint.Profiles, queryParams),
                cacheDuration ?? DefaultCacheDuration.UserProfile
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<Country>>> GetCountriesAsync(
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:countries", page ?? 1, perPage ?? 50);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<Country>>(SaphiApiEndpoint.Countries, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<Country>>(SaphiApiEndpoint.Countries, queryParams),
                cacheDuration ?? DefaultCacheDuration.Country
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<Character>>> GetCharactersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<SaphiApiResponse<Character>>(SaphiApiEndpoint.Characters)
            : await GetCachedAsync(
                "api:characters",
                () => GetAsync<SaphiApiResponse<Character>>(SaphiApiEndpoint.Characters),
                cacheDuration ?? DefaultCacheDuration.Character
            );

    public async Task<SaphiApiResult<SaphiApiResponse<EngineType>>> GetEngineTypesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<SaphiApiResponse<EngineType>>(SaphiApiEndpoint.Engines)
            : await GetCachedAsync(
                "api:engine_types",
                () => GetAsync<SaphiApiResponse<EngineType>>(SaphiApiEndpoint.Engines),
                cacheDuration ?? DefaultCacheDuration.Engine
            );

    public async Task<SaphiApiResult<SaphiApiResponse<Category>>> GetCategoriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<SaphiApiResponse<Category>>(SaphiApiEndpoint.Categories)
            : await GetCachedAsync(
                "api:categories",
                () => GetAsync<SaphiApiResponse<Category>>(SaphiApiEndpoint.Categories),
                cacheDuration ?? DefaultCacheDuration.Category
            );

    public async Task<SaphiApiResult<SaphiApiResponse<Player>>> GetPlayersAsync(
        int? id = null,
        string? username = null,
        int? countryId = null,
        int? statusId = null,
        string? discordId = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (id.HasValue)
            queryParams["id"] = id.Value;

        if (!string.IsNullOrWhiteSpace(username))
            queryParams["username"] = username;

        if (countryId.HasValue)
            queryParams["country_id"] = countryId.Value;

        if (statusId.HasValue)
            queryParams["status_id"] = statusId.Value;

        if (!string.IsNullOrWhiteSpace(discordId))
            queryParams["discord_id"] = discordId;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:players", id, username, countryId, statusId, discordId, page ?? 1, perPage ?? 100);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<Player>>(SaphiApiEndpoint.Players, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<Player>>(SaphiApiEndpoint.Players, queryParams),
                cacheDuration ?? DefaultCacheDuration.Players
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<Standard>>> GetStandardsAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false) =>
        forceRefresh
            ? await GetAsync<SaphiApiResponse<Standard>>(SaphiApiEndpoint.Standards)
            : await GetCachedAsync(
                "api:standards",
                () => GetAsync<SaphiApiResponse<Standard>>(SaphiApiEndpoint.Standards),
                cacheDuration ?? DefaultCacheDuration.Standard
            );

    public async Task<SaphiApiResult<SaphiApiResponse<SiteRecord>>> GetSiteRecordsAsync(
        int? categoryId = null,
        int? engineId = null,
        int? countryId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (categoryId.HasValue)
            queryParams["category_id"] = categoryId.Value;

        if (engineId.HasValue)
            queryParams["engine_id"] = engineId.Value;

        if (countryId.HasValue)
            queryParams["country_id"] = countryId.Value;

        var cacheKey = BuildCacheKey("api:site_records", categoryId, engineId, countryId);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<SiteRecord>>(SaphiApiEndpoint.SiteRecords, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<SiteRecord>>(SaphiApiEndpoint.SiteRecords, queryParams),
                cacheDuration ?? DefaultCacheDuration.SiteRecord
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<MatchupResult>>> GetMatchupAsync(
        int player1Id,
        int player2Id,
        string? categories = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>
        {
            { "player1_id", player1Id },
            { "player2_id", player2Id }
        };

        if (!string.IsNullOrWhiteSpace(categories))
            queryParams["categories"] = categories;

        var cacheKey = BuildCacheKey("api:matchup", player1Id, player2Id, categories);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<MatchupResult>>(SaphiApiEndpoint.Matchups, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<MatchupResult>>(SaphiApiEndpoint.Matchups, queryParams),
                cacheDuration ?? DefaultCacheDuration.Matchup
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<PointsRanking>>> GetPointsRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(type))
            queryParams["type"] = type;

        if (!string.IsNullOrWhiteSpace(category))
            queryParams["category"] = category;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:rankings:points", type, category, page ?? 1, perPage ?? 150);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<PointsRanking>>(SaphiApiEndpoint.RankingsPoints, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<PointsRanking>>(SaphiApiEndpoint.RankingsPoints, queryParams),
                cacheDuration ?? DefaultCacheDuration.Ranking
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<AverageFinishRanking>>> GetAverageFinishRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(type))
            queryParams["type"] = type;

        if (!string.IsNullOrWhiteSpace(category))
            queryParams["category"] = category;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:rankings:average_finish", type, category, page ?? 1, perPage ?? 150);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<AverageFinishRanking>>(SaphiApiEndpoint.RankingsAverageFinish, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<AverageFinishRanking>>(SaphiApiEndpoint.RankingsAverageFinish, queryParams),
                cacheDuration ?? DefaultCacheDuration.Ranking
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<AverageRankRanking>>> GetAverageRankRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(type))
            queryParams["type"] = type;

        if (!string.IsNullOrWhiteSpace(category))
            queryParams["category"] = category;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:rankings:average_rank", type, category, page ?? 1, perPage ?? 150);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<AverageRankRanking>>(SaphiApiEndpoint.RankingsAverageRank, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<AverageRankRanking>>(SaphiApiEndpoint.RankingsAverageRank, queryParams),
                cacheDuration ?? DefaultCacheDuration.Ranking
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<TotalTimeRanking>>> GetTotalTimeRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(type))
            queryParams["type"] = type;

        if (!string.IsNullOrWhiteSpace(category))
            queryParams["category"] = category;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:rankings:total_time", type, category, page ?? 1, perPage ?? 150);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<TotalTimeRanking>>(SaphiApiEndpoint.RankingsTotalTime, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<TotalTimeRanking>>(SaphiApiEndpoint.RankingsTotalTime, queryParams),
                cacheDuration ?? DefaultCacheDuration.Ranking
            );
    }

    public async Task<SaphiApiResult<SaphiApiResponse<SrPrRanking>>> GetSrPrRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false)
    {
        var queryParams = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(type))
            queryParams["type"] = type;

        if (!string.IsNullOrWhiteSpace(category))
            queryParams["category"] = category;

        if (page.HasValue)
            queryParams["page"] = page.Value;

        if (perPage.HasValue)
            queryParams["per_page"] = perPage.Value;

        var cacheKey = BuildCacheKey("api:rankings:sr_pr", type, category, page ?? 1, perPage ?? 150);

        return forceRefresh
            ? await GetAsync<SaphiApiResponse<SrPrRanking>>(SaphiApiEndpoint.RankingsSrPr, queryParams)
            : await GetCachedAsync(
                cacheKey,
                () => GetAsync<SaphiApiResponse<SrPrRanking>>(SaphiApiEndpoint.RankingsSrPr, queryParams),
                cacheDuration ?? DefaultCacheDuration.Ranking
            );
    }

    private string BuildCacheKey(string prefix, params object?[] parts)
    {
        if (parts.Length == 0)
            return prefix;

        var segments = parts.Select(p => p?.ToString() ?? "all");
        return $"{prefix}:{string.Join(":", segments)}";
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
