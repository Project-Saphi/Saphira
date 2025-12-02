using Saphira.Saphi.Api.Response;

namespace Saphira.Saphi.Api;

public interface ISaphiApiClient
{
    public Task<SaphiApiResult<GetCustomTrackResponse>> GetCustomTrackAsync(int trackId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetPlayerPBsResponse>> GetPlayerPBsAsync(int playerId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(int trackId, int categoryId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetRecentSubmissionsResponse>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        int? trackId = null,
        int? categoryId = null,
        int? userId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false
    );

    public Task<SaphiApiResult<GetUserProfileResponse>> GetUserProfileAsync(int userId, TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

    public Task<SaphiApiResult<GetPlayersResponse>> GetPlayersAsync(TimeSpan? cacheDuration = null, bool forceRefresh = false);

}
