using Saphira.Saphi.Api.Response;

namespace Saphira.Saphi.Api;

public interface ISaphiApiClient
{
    public Task<SaphiApiResult<GetCustomTrackResponse>> GetCustomTrackAsync(string trackId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetPlayerPBsResponse>> GetPlayerPBsAsync(string playerId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(string trackId, string categoryId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetRecentSubmissionsResponse>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        string? trackId = null,
        string? categoryId = null,
        string? userId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false
    );

    public Task<SaphiApiResult<GetUserProfileResponse>> GetUserProfileAsync(string userId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetPlayersResponse>> GetPlayersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

}
