namespace Saphira.Util.Game.Matchup;

public class PlayerMatchupCalculationResult(PlayerMatchupCalculationStatus status, string? errorMessage, PlayerMatchup? playerMatchup)
{
    public readonly PlayerMatchupCalculationStatus Status = status;
    public readonly string? ErrorMessage = errorMessage;
    public readonly PlayerMatchup? PlayerMatchup = playerMatchup;
}
