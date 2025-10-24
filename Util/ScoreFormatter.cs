namespace Saphira.Util
{
    public class ScoreFormatter
    {
        public string FormatScore(string score)
        {
            int hundreths = int.Parse(score.Substring(score.Length - 2));
            int seconds = int.Parse(score.Substring(0, score.Length - 2));
            int minutes = seconds / 60;

            return $"{minutes}:{seconds - (minutes * 60)}.{hundreths}";
        }
    }
}
