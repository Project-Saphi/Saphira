using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    public class TracksCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly CachedClient _client;

        public TracksCommand(CachedClient client)
        {
            _client = client;
        }

        [SlashCommand("tracks", "Get the list of supported custom tracks")]
        public async Task HandleCommand()
        {
            var result = await _client.GetCustomTracksAsync();

            if (!result.Success || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve custom track list: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var embed = new EmbedBuilder();

            var idsField = new EmbedFieldBuilder();
            idsField.WithName($":identification_card: {MessageTextFormat.Bold("ID")}");
            idsField.WithValue(String.Join("\n", GetIds(result.Response.Data)));
            idsField.WithIsInline(true);

            var customTracksField = new EmbedFieldBuilder();
            customTracksField.WithName($":motorway: {MessageTextFormat.Bold("Name")}");
            customTracksField.WithValue(String.Join("\n", GetCustomTrackNames(result.Response.Data)));
            customTracksField.WithIsInline(true);

            var authorsField = new EmbedFieldBuilder();
            authorsField.WithName($":art: {MessageTextFormat.Bold("Designer")}");
            authorsField.WithValue(String.Join("\n", GetAuthors(result.Response.Data)));
            authorsField.WithIsInline(true);

            embed.AddField(idsField);
            embed.AddField(customTracksField);
            embed.AddField(authorsField);

            await RespondAsync(embed: embed.Build());
        }

        private List<string> GetIds(List<CustomTrack> customTracks)
        {
            var ids = new List<string>();

            foreach (var customTrack in customTracks)
            {
                ids.Add($"#{customTrack.Id.ToString()}");
            }

            return ids;
        }

        private List<string> GetCustomTrackNames(List<CustomTrack> customTracks)
        {
            var customTrackNames = new List<string>();

            foreach (var customTrack in customTracks)
            {
                customTrackNames.Add(customTrack.Name);
            }

            return customTrackNames;
        }

        private List<string> GetAuthors(List<CustomTrack> customTracks)
        {
            var authors = new List<string>();

            foreach (var customTrack in customTracks)
            {
                authors.Add(customTrack.Author);
            }

            return authors;
        }
    }
}
