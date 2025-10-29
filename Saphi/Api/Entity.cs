using System.Text.Json.Serialization;

namespace Saphira.Saphi.Api
{
    public class CustomTrack
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("crc32_hash")]
        public string Crc32Hash { get; set; }

        [JsonPropertyName("has_files")]
        public string HasFiles { get; set; }

        [JsonPropertyName("file_1_name")]
        public string? File1Name { get; set; }

        [JsonPropertyName("file_2_name")]
        public string? File2Name { get; set; }

        [JsonPropertyName("standards")]
        public List<CustomTrackStandard>? Standards { get; set; }
    }

    public class CustomTrackStandard
    {
        [JsonPropertyName("tier_id")]
        public string TierId { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class PlayerPB
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("track_id")]
        public string TrackId { get; set; }

        [JsonPropertyName("track_name")]
        public string TrackName { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("holder")]
        public string Holder { get; set; }

        [JsonPropertyName("country_id")]
        public string CountryId { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }
    }

    public class UserProfile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("country")]
        public UserCountry Country { get; set; }

        [JsonPropertyName("stats")]
        public UserStats Stats { get; set; }
    }

    public class UserCountry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class UserStats
    {
        [JsonPropertyName("total_points")]
        public string TotalPoints { get; set; }

        [JsonPropertyName("course_points")]
        public string CoursePoints { get; set; }

        [JsonPropertyName("lap_points")]
        public string LapPoints { get; set; }

        [JsonPropertyName("tracks_submitted")]
        public string TracksSubmitted { get; set; }

        [JsonPropertyName("first_places")]
        public string FirstPlaces { get; set; }

        [JsonPropertyName("podium_finishes")]
        public string PodiumFinishes { get; set; }

        [JsonPropertyName("unique_tracks")]
        public string UniqueTracks { get; set; }

        [JsonPropertyName("course_tracks")]
        public string CourseTracks { get; set; }

        [JsonPropertyName("lap_tracks")]
        public string LapTracks { get; set; }

        [JsonPropertyName("first_submission")]
        public string? FirstSubmission { get; set; }

        [JsonPropertyName("last_submission")]
        public string? LastSubmission { get; set; }
    }

    public class TrackLeaderboardEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonPropertyName("character_id")]
        public string CharacterId { get; set; }

        [JsonPropertyName("country_id")]
        public string CountryId { get; set; }

        [JsonPropertyName("MinScore")]
        public string MinScore { get; set; }

        [JsonPropertyName("holder")]
        public string Holder { get; set; }
    }

    public class Country
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Character
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("default_engine")]
        public string DefaultEngine { get; set; }
    }

    public class EngineType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Category
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
