using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Core.Interaction.Autocompletion.ValueProvider;

namespace Saphira.Saphi.Interaction.Autocompletion.ValueProvider;

[AutoRegister]
public class RankingModeValueProvider : IValueProvider
{
    public static readonly Dictionary<int, string> RankingModes = new()
    {
        { 1, "player" },
        { 2, "country" }
    };

    public static readonly Dictionary<int, string> RankingModeDisplayNames = new()
    {
        { 1, "Player" },
        { 2, "Country" }
    };

    public Task<List<Value>> GetValuesAsync()
    {
        var values = RankingModeDisplayNames.Select(kvp => new Value(kvp.Key, kvp.Value)).ToList();
        return Task.FromResult(values);
    }

    public static string? GetRankingModeKey(int id) => RankingModes.GetValueOrDefault(id);
    public static string? GetRankingModeDisplayName(int id) => RankingModeDisplayNames.GetValueOrDefault(id);
}
