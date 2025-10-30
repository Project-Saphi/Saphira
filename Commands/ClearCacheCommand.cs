using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Caching.Memory;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    public class ClearCacheCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMemoryCache _cache;

        public ClearCacheCommand(IMemoryCache cache)
        {
            _cache = cache;
        }

        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [RequireTeamMemberRole]
        [SlashCommand("clearcache", "Clear the bot's in-memory cache")]
        public async Task HandleCommand()
        {
            await DeferAsync();

            try
            {
                // I should probably make a central registry for this ...
                // Otherwise it's too easy to forget to extend this
                var cacheKeys = new List<string>
                {
                    "categories",
                    "characters",
                    "custom_tracks",
                };

                int clearedCount = 0;
                foreach (var key in cacheKeys)
                {
                    _cache.Remove(key);
                    clearedCount++;
                }

                var successAlert = new SuccessAlertEmbedBuilder($"Successfully cleared {clearedCount} cache entry / entries.");
                await FollowupAsync(embed: successAlert.Build());
            }
            catch (Exception ex)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to clear cache: {ex.Message}");
                await FollowupAsync(embed: errorAlert.Build());
            }
        }
    }
}
