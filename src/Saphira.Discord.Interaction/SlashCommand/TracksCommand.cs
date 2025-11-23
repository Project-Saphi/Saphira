using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(30)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class TracksCommand(ApiClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 20;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/tracks", 
            $"{EntriesPerPage} entries are shown per page"
        );
    }

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

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("There are no custom tracks supported yet.");
            await RespondAsync(embed:  warningAlert.Build());
            return;
        }

        var customTracks = result.Response.Data;

        var paginationBuilder = new PaginationBuilder<CustomTrack>(paginationComponentHandler)
            .WithItems(customTracks)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageTracks, pageNumber) => GetEmbedForPage(pageTracks, pageNumber));

        var (embed, components) = paginationBuilder.Build();

        await RespondAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(List<CustomTrack> pageTracks, int pageNumber)
    {
        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}] Custom Track List");

        AddEmbedField(embed, ":identification_card:", "ID", GetIds(pageTracks));
        AddEmbedField(embed, ":motorway:", "Name", GetCustomTrackNames(pageTracks));
        AddEmbedField(embed, ":art:", "Designer", GetAuthors(pageTracks));

        return embed;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private List<string> GetIds(List<CustomTrack> tracks) =>
        [.. tracks.Select(t => $"#{t.Id}")];

    private List<string> GetCustomTrackNames(List<CustomTrack> tracks) =>
        [.. tracks.Select(t => t.Name)];

    private List<string> GetAuthors(List<CustomTrack> tracks) =>
        [.. tracks.Select(t => t.Author)];
}
