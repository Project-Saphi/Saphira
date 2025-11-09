using Discord;
using Discord.Interactions;
using Saphira.Commands.Metadata;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Extensions.Caching;
using Saphira.Util.Logging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
[RequireTeamMemberRole]
public class ClearCacheCommand(CacheInvalidationService cacheInvalidationService, IMessageLogger logger) : BaseCommand
{
    public override CommandMetadata GetMetadata()
    {
        return new CommandMetadata("/clearcache");
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("clearcache", "Invalidate all cached data")]
    public async Task HandleCommand()
    {
        await DeferAsync();

        try
        {
            cacheInvalidationService.InvalidateAll();

            var successAlert = new SuccessAlertEmbedBuilder("Successfully invalidated all cache entries. Data will be refreshed on next request.");
            await FollowupAsync(embed: successAlert.Build());

            logger.Log(LogSeverity.Info, Context.User.Username, "Cache invalidated");
        }
        catch (Exception ex)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to invalidate cache: {ex.Message}");
            await FollowupAsync(embed: errorAlert.Build());
        }
    }
}
