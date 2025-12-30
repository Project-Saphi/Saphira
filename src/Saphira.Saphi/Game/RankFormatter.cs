namespace Saphira.Saphi.Game;

public class RankFormatter
{
    public static string ToMedalFormat(int rank)
    {
        var medals = new[]
        {
            ":first_place:",
            ":second_place:",
            ":third_place:"
        };

        if (rank >= 1 && rank <= medals.Length)
        {
            return medals[rank - 1];
        }

        return $"#{rank}";
    }
}
