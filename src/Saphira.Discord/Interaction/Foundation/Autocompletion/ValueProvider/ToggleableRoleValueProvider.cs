using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Entity.Guild.Role;

namespace Saphira.Discord.Interaction.Foundation.Autocompletion.ValueProvider;

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
