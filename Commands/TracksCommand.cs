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
    public class TracksCommand(CachedClient client) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("tracks", "Get the list of supported custom tracks")]
        public async Task HandleCommand()
        {
            var result = await client.GetCustomTracksAsync();

            if (!result.Success || result.Response == null)
            {
                var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve custom track list: {result.ErrorMessage ?? "Unknown error"}");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }

            var embed = new EmbedBuilder();

            AddEmbedField(embed, ":identification_card:", "ID", GetIds(result.Response.Data));
            AddEmbedField(embed, ":motorway:", "Name", GetCustomTrackNames(result.Response.Data));
            AddEmbedField(embed, ":art:", "Designer", GetAuthors(result.Response.Data));

            await RespondAsync(embed: embed.Build());
        }

        private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
        {
            embed.AddField(new EmbedFieldBuilder()
                .WithName($"{emote} {MessageTextFormat.Bold(title)}")
                .WithValue(string.Join("\n", content))
                .WithIsInline(true));
        }

        private List<string> GetIds(List<CustomTrack> tracks) =>
            tracks.Select(t => $"#{t.Id}").ToList();

        private List<string> GetCustomTrackNames(List<CustomTrack> tracks) =>
            tracks.Select(t => t.Name).ToList();

        private List<string> GetAuthors(List<CustomTrack> tracks) =>
            tracks.Select(t => t.Author).ToList();
    }
}
