using Discord;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Discord.Logging;
using Saphira.Saphi.Api;

namespace Saphira.Discord.Interaction.Autocompletion.ValueProvider;

[AutoRegister]
public class CustomTrackValueProvider(CachedClient client, IMessageLogger logger) : IValueProvider
{
    public async Task<List<Value>> GetValuesAsync()
    {
        var result = await client.GetCustomTracksAsync();

        if (!result.Success || result.Response == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch custom tracks: {result.ErrorMessage ?? "Unknown error"}");
            return [];
        }

        return [.. result.Response.Data.Select(ct => new Value(int.Parse(ct.Id), ct.Name))];
    }
}
