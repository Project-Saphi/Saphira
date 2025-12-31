using Discord;
using Saphira.Core.Extensions.DependencyInjection;
using Saphira.Core.Logging;
using Saphira.Discord.Core.Interaction.Autocompletion.ValueProvider;
using Saphira.Saphi.Api;

namespace Saphira.Saphi.Interaction.Autocompletion.ValueProvider;

[AutoRegister]
public class CustomTrackValueProvider(ISaphiApiClient client, IMessageLogger logger) : IValueProvider
{
    public async Task<List<Value>> GetValuesAsync()
    {
        var result = await client.GetCustomTracksAsync();

        if (!result.Success || result.Response == null)
        {
            logger.Log(LogSeverity.Error, "Saphira", $"Failed to fetch custom tracks: {result.ErrorMessage ?? "Unknown error"}");
            return [];
        }

        return [.. result.Response.Data.Select(ct => new Value(ct.Id, ct.Name))];
    }
}
