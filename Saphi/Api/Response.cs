using System.Text.Json.Serialization;

namespace Saphira.Saphi.Api
{
    public class GetCustomTracksResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public List<CustomTrack> Data { get; set; }
    }

    public class GetCustomTrackResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public CustomTrack Data { get; set; }
    }

    public class GetTrackLeaderboardResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public List<TrackLeaderboardEntry> Data { get; set; }
    }

    public class GetPlayerPBsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public List<PlayerPB> Data { get; set; }
    }

    public class GetUserProfileResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public UserProfile Data { get; set; }
    }
}
