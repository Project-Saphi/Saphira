using Discord;
using Discord.Interactions;
using Saphira.Discord.Core.Interaction.Autocompletion;
using Saphira.Discord.Core.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Discord.Pagination;
using Saphira.Discord.Pagination.Builder;
using Saphira.Discord.Pagination.Component;
using Saphira.Saphi.Api;
using Saphira.Saphi.Core.Entity.Leaderboard;
using Saphira.Saphi.Interaction.Autocompletion.ValueProvider;
using System.Text;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class SiteRecordsCommand(ISaphiApiClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 15;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/site-records",
            $"{EntriesPerPage} entries are shown per page. Optionally filter by category."
        );
    }

    [SlashCommand("site-records", "Get the current site records (world records)")]
    public async Task HandleCommand(
        [Autocomplete(typeof(GenericAutocompleteHandler<CategoryValueProvider>))] int? category = null
        )
    {
        await DeferAsync();

        var result = await client.GetSiteRecordsAsync(categoryId: category);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve site records: {result.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("No site records found.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var siteRecords = result.Response.Data;

        var paginationBuilder = new ListPaginationBuilder<SiteRecord>(paginationComponentHandler)
            .WithItems(siteRecords)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageRecords, pageNumber) => GetEmbedForPage(pageRecords, pageNumber, category))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();

        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(List<SiteRecord> pageRecords, int pageNumber, int? categoryFilter)
    {
        if (pageRecords.Count == 0)
        {
            return new EmbedBuilder().WithDescription("No entries found.");
        }

        var recordData = GetRecordData(pageRecords);
        var categoryName = categoryFilter.HasValue ? pageRecords.First().CategoryName : "All Categories";

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}] Site Records ({categoryName})");

        AddEmbedField(embed, ":motorway:", "Track", recordData["tracks"]);
        AddEmbedField(embed, ":stopwatch:", "Time", recordData["times"]);
        AddEmbedField(embed, ":bust_in_silhouette:", "Record Holder", recordData["holders"]);

        return embed;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private Dictionary<string, List<string>> GetRecordData(List<SiteRecord> records)
    {
        var dict = new Dictionary<string, List<string>>()
        {
            { "tracks", [] },
            { "times", [] },
            { "holders", [] }
        };

        foreach (var record in records)
        {
            dict["tracks"].Add(BuildTrackString(record));
            dict["times"].Add(BuildTimeString(record));
            dict["holders"].Add(BuildHolderString(record));
        }

        return dict;
    }

    private string BuildTrackString(SiteRecord record)
    {
        var trackString = new StringBuilder();

        if (record.StandardName != null)
        {
            var standardEmote = TierEmoteMapper.MapTierToEmote(record.StandardId?.ToString() ?? "");
            if (standardEmote != null)
            {
                trackString.Append(standardEmote).Append(' ');
            }
        }

        trackString.Append($"{MessageTextFormat.Bold($"{record.TrackName}")} ({record.CategoryName})");

        return trackString.ToString();
    }

    private string BuildTimeString(SiteRecord record)
    {
        return new StringBuilder()
            .Append(CharacterEmoteMapper.MapCharacterToEmote(record.CharacterName))
            .Append(' ')
            .Append(record.TimeFormatted)
            .ToString();
    }

    private string BuildHolderString(SiteRecord record)
    {
        return new StringBuilder()
            .Append(CountryEmoteMapper.MapCountryToEmote(record.CountryName))
            .Append(' ')
            .Append(record.Name)
            .ToString();
    }
}
