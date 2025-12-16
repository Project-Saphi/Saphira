using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Game.Matchup;

public class PlayerMatchupCalculator(ISaphiApiClient client)
{
    public async Task<PlayerMatchupCalculationResult> GeneratePlayerMatchup(int player1, int player2, int category)
    {
        if (player1 == player2)
        {
            return new PlayerMatchupCalculationResult(PlayerMatchupCalculationStatus.Failure, "You cannot compare a player against himself.", null);
        }

        var categoryEntity = await FetchCategory(category);

        if (categoryEntity == null)
        {
            return new PlayerMatchupCalculationResult(PlayerMatchupCalculationStatus.Failure, "The specified category does not exist.", null);
        }

        var player1Profile = await FetchPlayerProfileAsync(player1);
        var player2Profile = await FetchPlayerProfileAsync(player2);

        if (player1Profile == null || player2Profile == null)
        {
            return new PlayerMatchupCalculationResult(PlayerMatchupCalculationStatus.Failure, "One or more players don't exist.", null);
        }

        var player1PBs = await FetchPlayerPBsAsync(player1);
        var player2PBs = await FetchPlayerPBsAsync(player2);

        if (player1PBs == null || player2PBs == null)
        {
            return new PlayerMatchupCalculationResult(PlayerMatchupCalculationStatus.Failure, "Failed to fetch player PBs.", null);
        }

        var playerMatchup = ComparePlayers(player1Profile, player2Profile, player1PBs, player2PBs, categoryEntity);

        if (playerMatchup.PlayerRecordComparisons.Count == 0)
        {
            return new PlayerMatchupCalculationResult(PlayerMatchupCalculationStatus.Failure, "These players have no tracks in common.", null);
        }

        return new PlayerMatchupCalculationResult(PlayerMatchupCalculationStatus.Success, null, playerMatchup);
    }

    private PlayerMatchup ComparePlayers(UserProfile player1Profile, UserProfile player2Profile, List<PlayerPB> player1PBs, List<PlayerPB> player2PBs, Category categoryEntity)
    {
        var playerRecordComparisons = new List<PlayerRecordComparison>();

        var player1PBDict = player1PBs
            .Where(pb => pb.CategoryId == categoryEntity.Id)
            .ToDictionary(pb => pb.TrackName, pb => pb);

        var player2PBDict = player2PBs
            .Where(pb => pb.CategoryId == categoryEntity.Id)
            .ToDictionary(pb => pb.TrackName, pb => pb);

        var commonTracks = player1PBDict.Keys.Intersect(player2PBDict.Keys).ToList();

        var player1Name = player1Profile.Name;
        var player2Name = player2Profile.Name;
        var country1Name = player1Profile.Country?.Name ?? string.Empty;
        var country2Name = player2Profile.Country?.Name ?? string.Empty;

        int player1Wins = 0;
        int player2Wins = 0;

        foreach (var track in commonTracks)
        {
            var player1PB = player1PBDict[track];
            var player2PB = player2PBDict[track];

            var player1TimeInSeconds = player1PB.Time;
            var player2TimeInSeconds = player2PB.Time;

            PlayerRecordComparison comparison;

            if (player1TimeInSeconds < player2TimeInSeconds)
            {
                comparison = new PlayerRecordComparison(
                    track,
                    player1Name,
                    player1PB.TimeFormatted,
                    player2Name,
                    player2PB.TimeFormatted
                );
                player1Wins++;
            }
            else if (player2TimeInSeconds < player1TimeInSeconds)
            {
                comparison = new PlayerRecordComparison(
                    track,
                    player2Name,
                    player2PB.TimeFormatted,
                    player1Name,
                    player1PB.TimeFormatted
                );
                player2Wins++;
            }
            else
            {
                comparison = new PlayerRecordComparison(
                    track,
                    null,
                    player1PB.TimeFormatted,
                    null,
                    player2PB.TimeFormatted
                );
            }

            playerRecordComparisons.Add(comparison);
        }

        string? winnerName = null;
        string? loserName = null;
        int? winnerWins = null;
        int? loserWins = null;

        if (player1Wins > player2Wins)
        {
            winnerName = player1Name;
            winnerWins = player1Wins;

            loserName = player2Name;
            loserWins = player2Wins;
        }
        else if (player2Wins > player1Wins)
        {
            winnerName = player2Name;
            winnerWins = player2Wins;

            loserName = player1Name;
            loserWins = player1Wins;
        }

        return new PlayerMatchup(
            player1Name,
            country1Name,
            player2Name,
            country2Name,
            winnerName,
            loserName,
            winnerWins,
            loserWins,
            categoryEntity,
            playerRecordComparisons
        );
    }

    private async Task<UserProfile?> FetchPlayerProfileAsync(int userId)
    {
        var result = await client.GetUserProfileAsync(userId);

        if (!result.Success || result.Response == null || result.Response.Data.Count == 0)
        {
            return null;
        }

        return result.Response.Data.FirstOrDefault();
    }

    private async Task<List<PlayerPB>?> FetchPlayerPBsAsync(int playerId)
    {
        var result = await client.GetPlayerPBsAsync(playerId);

        if (!result.Success || result.Response == null)
        {
            return null;
        }

        return result.Response.Data;
    }

    private async Task<Category?> FetchCategory(int categoryId)
    {
        var result = await client.GetCategoriesAsync();

        if (!result.Success || result.Response == null)
        {
            return null;
        }

        return result.Response.Data.Where(c => c.Id == categoryId).FirstOrDefault();
    }
}
