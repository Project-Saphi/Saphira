using Discord;
using Saphira.Extensions.DependencyInjection;
using Saphira.Saphi.Api;
using Saphira.Util.Logging;

namespace Saphira.Commands.Autocompletion.ValueProvider;

[AutoRegister]
public class PlayerValueProvider(CachedClient client, IMessageLogger logger) : IValueProvider
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
