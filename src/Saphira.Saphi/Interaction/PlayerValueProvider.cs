using Discord;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Discord.Interaction.Foundation.Autocompletion.ValueProvider;
using Saphira.Saphi.Api;

namespace Saphira.Saphi.Interaction;

[AutoRegister]
public class PlayerValueProvider(ApiClient client, IMessageLogger logger) : IValueProvider
{
    public async Task<List<Value>> GetValuesAsync()
    {
        var result = await client.GetPlayersAsync();

        if (!result.Success || result.Response == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch players: {result.ErrorMessage ?? "Unknown error"}");
            return [];
        }

        return [.. result.Response.Data.Select(p => new Value(p.Id, p.Username))];
    }
}
