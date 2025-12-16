using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Pagination;
using Saphira.Discord.Pagination.Builder;
using Saphira.Discord.Pagination.Component;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(30)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class TracksCommand(ISaphiApiClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
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
        await DeferAsync();

        var customTracksResult = await client.GetCustomTracksAsync();

        if (!customTracksResult.Success || customTracksResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve custom track list: {customTracksResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (customTracksResult.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("There are no custom tracks supported yet.");
            await FollowupAsync(embed:  warningAlert.Build());
            return;
        }

        var customTracks = customTracksResult.Response.Data;

        var paginationBuilder = new ListPaginationBuilder<CustomTrack>(paginationComponentHandler)
            .WithItems(customTracks)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageTracks, pageNumber) => GetEmbedForPage(pageTracks, pageNumber))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();

        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(List<CustomTrack> pageTracks, int pageNumber)
    {
        var customTracksData = GetCustomTracksData(pageTracks);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}] Custom Track List");

        AddEmbedField(embed, ":identification_card:", "ID", customTracksData["ids"]);
        AddEmbedField(embed, ":motorway:", "Name", customTracksData["names"]);
        AddEmbedField(embed, ":art:", "Designer", customTracksData["creators"]);

        return embed;
    }

    private Dictionary<string, List<string>> GetCustomTracksData(List<CustomTrack> customTracks)
    {
        var data = new Dictionary<string, List<string>>()
        {
            { "ids", [] },
            { "names", [] },
            { "creators", [] }
        };

        foreach (var track in customTracks)
        {
            data["ids"].Add($"#{track.Id}");
            data["names"].Add($"{MessageTextFormat.Bold(track.Name)} ({track.SubmissionCount})");
            data["creators"].Add(track.Author);
        }

        return data;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }
}
