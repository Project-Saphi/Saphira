using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Game.Matchup;

public class PlayerMatchup(
    string playerName1,
    string countryName1,
    string playerName2, 
    string countryName2,
    string? winnerName, 
    string? loserName, 
    int? winnerWins, 
    int? loserWins,
    Category category, 
    List<PlayerRecordComparison> playerRecordComparisons
    )
{
    public readonly string PlayerName1 = playerName1;
    public readonly string CountryName1 = countryName1;
    public readonly string PlayerName2 = playerName2;
    public readonly string CountryName2 = countryName2;
    public readonly string? WinnerName = winnerName;
    public readonly string? LoserName = loserName;
    public readonly int? WinnerWins = winnerWins;
    public readonly int? LoserWins = loserWins;
    public readonly Category Category = category;
    public readonly List<PlayerRecordComparison> PlayerRecordComparisons = playerRecordComparisons;

    public bool IsEvenMatchup()
    {
        return WinnerName == null && LoserName == null;
    }
}
