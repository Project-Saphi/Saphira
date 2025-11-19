using Saphira.Saphi.Api.Response;

namespace Saphira.Saphi.Api;

public interface ISaphiApiClient
{
    public Task<Result<GetCustomTracksResponse>> GetCustomTracksAsync(TimeSpan? cacheDuration = null);

    public Task<Result<GetPlayerPBsResponse>> GetPlayerPBsAsync(string playerId, TimeSpan? cacheDuration = null);

    public Task<Result<GetTrackLeaderboardResponse>> GetTrackLeaderboardAsync(string trackId, string categoryId, TimeSpan? cacheDuration = null);

    public Task<Result<GetRecentSubmissionsResponse>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        string? trackId = null,
        string? categoryId = null,
        string? userId = null,
        TimeSpan? cacheDuration = null
    );

    public Task<Result<GetUserProfileResponse>> GetUserProfileAsync(string userId, TimeSpan? cacheDuration = null);

    public Task<Result<GetCountriesResponse>> GetCountriesAsync(TimeSpan? cacheDuration = null);

    public Task<Result<GetCharactersResponse>> GetCharactersAsync(TimeSpan? cacheDuration = null);

    public Task<Result<GetEngineTypesResponse>> GetEngineTypesAsync(TimeSpan? cacheDuration = null);

    public Task<Result<GetCategoriesResponse>> GetCategoriesAsync(TimeSpan? cacheDuration = null);

    public Task<Result<GetPlayersResponse>> GetPlayersAsync(TimeSpan? cacheDuration = null);

}
