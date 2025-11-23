using Saphira.Saphi.Api.Response;

namespace Saphira.Saphi.Api;

public interface IApiClient
{
    public Task<ApiResult<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetPlayerPBsResponse>> GetPlayerPBsAsync(string playerId, TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(string trackId, string categoryId, TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetRecentSubmissionsResponse>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        string? trackId = null,
        string? categoryId = null,
        string? userId = null,
        TimeSpan? cacheDuration = null
    );

    public Task<ApiResult<GetUserProfileResponse>> GetUserProfileAsync(string userId, TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null);

    public Task<ApiResult<GetPlayersResponse>> GetPlayersAsync(TimeSpan? cacheDuration = null);

}
