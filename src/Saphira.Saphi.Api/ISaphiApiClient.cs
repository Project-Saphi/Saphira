using Saphira.Saphi.Api.Response;
using Saphira.Saphi.Core.Entity;
using Saphira.Saphi.Core.Entity.Leaderboard;
using Saphira.Saphi.Core.Entity.Ranking;
using Saphira.Saphi.Core.Entity.Reference;
using Saphira.Saphi.Core.Entity.User;

namespace Saphira.Saphi.Api;

public interface ISaphiApiClient
{
    Task<SaphiApiResult<SaphiApiResponse<CustomTrack>>> GetCustomTrackAsync(
        int trackId,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<CustomTrack>>> GetCustomTracksAsync(
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<PlayerPB>>> GetPlayerPBsAsync(
        int userId,
        int? trackId = null,
        int? categoryId = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<PlayerPB>>> GetPlayerPBsByUsernameAsync(
        string username,
        int? trackId = null,
        int? categoryId = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<TrackLeaderboardEntry>>> GetTrackLeaderboardAsync(
        int trackId,
        int categoryId,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<RecentSubmission>>> GetRecentSubmissionsAsync(
        string? timeFilter = null,
        int? trackId = null,
        int? categoryId = null,
        int? userId = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<UserProfile>>> GetUserProfileAsync(
        int userId,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<UserProfile>>> GetUserProfileByUsernameAsync(
        string username,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<Country>>> GetCountriesAsync(
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<Character>>> GetCharactersAsync(
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<EngineType>>> GetEngineTypesAsync(
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<Category>>> GetCategoriesAsync(
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<Standard>>> GetStandardsAsync(
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<Player>>> GetPlayersAsync(
        int? id = null,
        string? username = null,
        int? countryId = null,
        int? statusId = null,
        string? discordId = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<SiteRecord>>> GetSiteRecordsAsync(
        int? categoryId = null,
        int? engineId = null,
        int? countryId = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<MatchupResult>>> GetMatchupAsync(
        int player1Id,
        int player2Id,
        string? categories = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<PointsRanking>>> GetPointsRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<AverageFinishRanking>>> GetAverageFinishRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<AverageRankRanking>>> GetAverageRankRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<TotalTimeRanking>>> GetTotalTimeRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);

    Task<SaphiApiResult<SaphiApiResponse<SrPrRanking>>> GetSrPrRankingsAsync(
        string? type = null,
        string? category = null,
        int? page = null,
        int? perPage = null,
        TimeSpan? cacheDuration = null,
        bool forceRefresh = false);
}
