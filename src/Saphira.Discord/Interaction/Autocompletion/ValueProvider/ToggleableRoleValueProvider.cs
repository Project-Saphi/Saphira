using Saphira.Discord.Guild;
using Saphira.Discord.Extensions.DependencyInjection;

namespace Saphira.Discord.Interaction.Autocompletion.ValueProvider;

[AutoRegister]
public class ToggleableRoleValueProvider(GuildRoleManager guildRoleManager) : IValueProvider
{
    public Task<List<Value>> GetValuesAsync()
    {
        var values = guildRoleManager.GetToggleableRoles()
            .Select((role, index) => new Value(index, role))
            .ToList();

        return Task.FromResult(values);
    }
}
