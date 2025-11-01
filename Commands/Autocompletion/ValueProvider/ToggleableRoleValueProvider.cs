using Saphira.Discord.Guild;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
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
}
