namespace Saphira.Saphi.Game;

public class ScoreFormatter
{
    public static string AsHumanTime(int score)
    {
        int hundreths = score % 100;
        int totalSeconds = score / 100;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{(minutes <= 0 ? "" : $"{minutes}:")}{seconds:00}.{hundreths:00}";
    }

    public static string AsHumanTime(string score)
    {
        return AsHumanTime(int.Parse(score));
    }
}
