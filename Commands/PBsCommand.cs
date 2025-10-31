using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    public class PBsCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly CachedClient _client;

        public PBsCommand(CachedClient client)
        {
            _client = client;
        }

        [SlashCommand("pbs", "Get personal best times of a player")]
        public async Task HandleCommand(string player)
        {
            var result = await _client.GetPlayerPBsAsync(player);

            if (!result.Success || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve personal best times: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var playerName = result.Response.Data.First().Holder;

            var embed = new EmbedBuilder();

            var customTracksField = new EmbedFieldBuilder();
            customTracksField.WithName($":motorway: {MessageTextFormat.Bold("Track")}");
            customTracksField.WithValue(String.Join("\n", GetCustomTracks(result.Response.Data)));
            customTracksField.WithIsInline(true);

            var timesField = new EmbedFieldBuilder();
            timesField.WithName($":stopwatch: {MessageTextFormat.Bold("Time")}");
            timesField.WithValue(String.Join("\n", GetTimes(result.Response.Data)));
            timesField.WithIsInline(true);

            var placementsField = new EmbedFieldBuilder();
            placementsField.WithName($":trophy: {MessageTextFormat.Bold("Placement")}");
            placementsField.WithValue(String.Join("\n", GetPlacements(result.Response.Data)));
            placementsField.WithIsInline(true);

            embed.AddField(customTracksField);
            embed.AddField(timesField);
            embed.AddField(placementsField);

            await RespondAsync(embed: embed.Build());
        }

        private List<string> GetCustomTracks(List<PlayerPB> playerPBs)
        {
            var customTracks = new List<string>();

            foreach (var playerPB in playerPBs)
            {
                customTracks.Add(MessageTextFormat.Bold(playerPB.TrackName));
            }

            return customTracks;
        }

        private List<string> GetTimes(List<PlayerPB> playerPBs)
        {
            var times = new List<string>();

            foreach (var playerPB in playerPBs)
            {
                times.Add(ScoreFormatter.AsIngameTime(playerPB.Time));
            }

            return times;
        }

        private List<string> GetPlacements(List<PlayerPB> playerPBs)
        {
            var placements = new List<string>();

            foreach (var playerPB in playerPBs)
            {
                placements.Add(RankFormatter.ToMedalFormat(int.Parse(playerPB.Rank)));
            }

            return placements;
        }

        private async Task<Category?> FindCategory(string categoryId)
        {
            var result = await _client.GetCategoriesAsync();

            if (!result.Success || result.Response == null)
            {
                return null;
            }

            return result.Response.Data.FirstOrDefault(category => category.Id == categoryId);
        }
    }
}
