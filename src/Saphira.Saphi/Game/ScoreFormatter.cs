namespace Saphira.Saphi.Game;

public class ScoreFormatter
{
    public static string AsHumanTime(string score)
    {
        int hundreths = int.Parse(score[^2..]);
        int seconds = int.Parse(score[..^2]);
        int minutes = seconds / 60;

        return $"{(minutes <= 0 ? "" : $"{minutes}:")}{seconds - minutes * 60:00}.{hundreths:00}";
    }
}
