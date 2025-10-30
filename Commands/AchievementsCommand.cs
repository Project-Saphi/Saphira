using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;

namespace Saphira.Commands
{
    public class AchievementsCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Client _client;

        public AchievementsCommand(Client client)
        {
            _client = client;
        }

        [RequireTextChannel]
        [SlashCommand("achievements", "Show a player's achievements")]
        public async Task HandleCommand(string player)
        {
            var result = await _client.GetUserProfileAsync(player);

            if (result.Success == false || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve user profile of {player}: {result.ErrorMessage ?? "Unknown error"}");
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
