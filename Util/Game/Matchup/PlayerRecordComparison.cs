namespace Saphira.Util.Game.Matchup;

public class PlayerRecordComparison(string trackName, string? winnerName, string winnerTime, string? loserName, string loserTime)
{
    public readonly string TrackName = trackName;
    public readonly string? WinnerName = winnerName;
    public readonly string WinnerTime = winnerTime;
    public readonly string? LoserName = loserName;
    public readonly string LoserTime = loserTime;

    public bool IsEvenRecord()
    {
        return WinnerName == null && LoserName == null;
    }
}
