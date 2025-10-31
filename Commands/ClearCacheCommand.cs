using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Extensions.Caching;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    [RequireTeamMemberRole]
    public class ClearCacheCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly CacheInvalidationService _cacheInvalidationService;

        public ClearCacheCommand(CacheInvalidationService cacheInvalidationService)
        {
            _cacheInvalidationService = cacheInvalidationService;
        }

        [CommandContextType(InteractionContextType.Guild)]
        [SlashCommand("clearcache", "Invalidate all cached data")]
        public async Task HandleCommand()
        {
            await DeferAsync();

            try
            {
                _cacheInvalidationService.InvalidateAll();

                var successAlert = new SuccessAlertEmbedBuilder("Successfully invalidated all cache entries. Data will be refreshed on next request.");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to invalidate cache: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
