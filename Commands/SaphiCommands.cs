using Discord.Interactions;
using Saphira.Util;
using System.Text.Json.Serialization;

namespace Saphira.Commands
{
    public class LeaderboardEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

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

    public class LeaderboardResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public List<LeaderboardEntry> Data { get; set; }
    }

    public class SaphiCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private SaphiClient _saphiClient = null!;
        private ScoreFormatter _scoreFormatter = null!;

        public SaphiCommands(SaphiClient saphiClient, ScoreFormatter scoreFormatter)
        {
            _saphiClient = saphiClient;
            _scoreFormatter = scoreFormatter;
        }

        [SlashCommand("leaderboard", "Get the leaderboard for a single track")]
        public async Task LeaderboardCommand(string trackId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "id", trackId },
                { "type", "1" }
            };

            var response = await _saphiClient.GetAsync<LeaderboardResponse>("track-leaderboards.php", queryParams);

            if (response?.Success == true)
            {
                // Example: Access the data
                var topPlayer = response.Data.FirstOrDefault();
                if (topPlayer != null)
                {
                    await RespondAsync($"Top player: {topPlayer.Holder} with score {_scoreFormatter.FormatScore(topPlayer.MinScore)}");
                }
            }
            else
            {
                await RespondAsync($"Failed to retrieve leaderboard: {response?.Message}");
            }
        }
    }
}
