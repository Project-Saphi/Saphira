using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Core.Interaction.Autocompletion.ValueProvider;

namespace Saphira.Saphi.Interaction.Autocompletion.ValueProvider;

[AutoRegister]
public class RankingTypeValueProvider : IValueProvider
{
    public static readonly Dictionary<int, string> RankingTypes = new()
    {
        { 1, "points" },
        { 2, "average-finish" },
        { 3, "average-rank" },
        { 4, "total-time" },
        { 5, "sr-pr" }
    };

    public static readonly Dictionary<int, string> RankingTypeDisplayNames = new()
    {
        { 1, "Points" },
        { 2, "Average Finish" },
        { 3, "Average Rank" },
        { 4, "Total Time" },
        { 5, "SR:PR" }
    };

    public Task<List<Value>> GetValuesAsync()
    {
        var values = RankingTypeDisplayNames.Select(kvp => new Value(kvp.Key, kvp.Value)).ToList();
        return Task.FromResult(values);
    }

    public static string? GetRankingTypeKey(int id) => RankingTypes.GetValueOrDefault(id);
    public static string? GetRankingTypeDisplayName(int id) => RankingTypeDisplayNames.GetValueOrDefault(id);
}
