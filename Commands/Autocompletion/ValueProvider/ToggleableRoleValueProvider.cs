using Saphira.Discord.Guild;

namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class ToggleableRoleValueProvider : IValueProvider
    {
        private readonly GuildRoleManager _guildRoleManager;

        public ToggleableRoleValueProvider(GuildRoleManager guildRoleManager)
        {
            _guildRoleManager = guildRoleManager;
        }

        public Task<List<Value>> GetValuesAsync()
        {
            var toggleableRoles = _guildRoleManager.GetToggleableRoles();
            var values = new List<Value>();

            var index = 0;
            foreach (var role in toggleableRoles)
            {
                values.Add(new Value(index, role));
                index++;
            }

            return Task.FromResult(values);
        }
    }
}
