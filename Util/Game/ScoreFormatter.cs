namespace Saphira.Util.Game
{
    public class ScoreFormatter
    {
        public static string AsIngameTime(string score)
        {
            int hundreths = int.Parse(score[^2..]);
            int seconds = int.Parse(score[..^2]);
            int minutes = seconds / 60;

            return $"{minutes}:{seconds - minutes * 60:00}.{hundreths:00}";
        }

        public static string AsSeconds(string score)
        {
            int hundreths = int.Parse(score[^2..]);
            int seconds = int.Parse(score[..^2]);

            return $"{seconds}.{hundreths}";
        }
    }
}
