using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    public class AchievementsCommand(CachedClient client) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("achievements", "Show a player's achievements")]
        public async Task HandleCommand(string player)
        {
            var result = await client.GetUserProfileAsync(player);

            if (!result.Success || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve user profile: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var stats = result.Response.Data.Stats;

            var content = new[]
            {
                $"{MessageTextFormat.Bold("Total Points")}: {stats.TotalPoints}",
                $"{MessageTextFormat.Bold("Course Points")}: {stats.CoursePoints}",
                $"{MessageTextFormat.Bold("Lap Points")}: {stats.LapPoints}",
                $"{MessageTextFormat.Bold("First Places")}: {stats.FirstPlaces}",
                $"{MessageTextFormat.Bold("Podium Finishes")}: {stats.PodiumFinishes}"
            };

            var embed = new EmbedBuilder();
            
            var field = new EmbedFieldBuilder();
            field.WithName($"{result.Response.Data.Username}'s Achievements");
            field.WithValue(String.Join("\n", content));

            embed.AddField(field);

            await RespondAsync(embed: embed.Build());
        }
    }
}
