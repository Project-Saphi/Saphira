using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Core.Entity.Guild.Role;

namespace Saphira.Discord.Core.Interaction.Autocompletion.ValueProvider;

[AutoRegister]
public class ToggleableRoleValueProvider : IValueProvider
{
    public static readonly Dictionary<int, string> ToggleableRoles = new()
    {
        { 1, GuildRole.SaphiUpdatesRole },
        { 2, GuildRole.ServerUpdatesRole },
        { 3, GuildRole.WRFeedRole }
    };

    public Task<List<Value>> GetValuesAsync()
    {
        var values = ToggleableRoles.Select(kvp => new Value(kvp.Key, kvp.Value)).ToList();
        return Task.FromResult(values);
    }

    public static string? GetToggleableRole(int id) => ToggleableRoles.GetValueOrDefault(id);
}
