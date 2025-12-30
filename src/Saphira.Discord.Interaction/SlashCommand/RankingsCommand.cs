using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Foundation.Autocompletion;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Discord.Pagination;
using Saphira.Discord.Pagination.Builder;
using Saphira.Discord.Pagination.Component;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity.Ranking;
using Saphira.Saphi.Game;
using Saphira.Saphi.Interaction;
using System.Globalization;
using System.Text;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class RankingsCommand(ISaphiApiClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 15;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/rankings Points Player",
            $"{EntriesPerPage} entries are shown per page"
        );
    }

    [SlashCommand("rankings", "View the leaderboard rankings")]
    public async Task HandleCommand(
        [Autocomplete(typeof(GenericAutocompleteHandler<RankingTypeValueProvider>))] int rankingType,
        [Autocomplete(typeof(GenericAutocompleteHandler<RankingModeValueProvider>))] int mode = 1
        )
    {
        await DeferAsync();

        var rankingTypeKey = RankingTypeValueProvider.GetRankingTypeKey(rankingType);
        var rankingModeKey = RankingModeValueProvider.GetRankingModeKey(mode);
        var rankingTypeDisplayName = RankingTypeValueProvider.GetRankingTypeDisplayName(rankingType);
        var rankingModeDisplayName = RankingModeValueProvider.GetRankingModeDisplayName(mode);

        if (rankingTypeKey == null || rankingModeKey == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder("Invalid ranking type or mode.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var title = $"{rankingTypeDisplayName} Rankings ({rankingModeDisplayName})";
        var isCountryRanking = rankingModeKey == "country";

        switch (rankingTypeKey)
        {
            case "points":
                await HandlePointsRanking(rankingModeKey, title, isCountryRanking);
                break;
            case "average-finish":
                await HandleAverageFinishRanking(rankingModeKey, title, isCountryRanking);
                break;
            case "average-rank":
                await HandleAverageRankRanking(rankingModeKey, title, isCountryRanking);
                break;
            case "total-time":
                await HandleTotalTimeRanking(rankingModeKey, title, isCountryRanking);
                break;
            case "sr-pr":
                await HandleSrPrRanking(rankingModeKey, title, isCountryRanking);
                break;
            default:
                var errorAlert = new ErrorAlertEmbedBuilder("Unknown ranking type.");
                await FollowupAsync(embed: errorAlert.Build());
                break;
        }
    }

    private async Task HandlePointsRanking(string type, string title, bool isCountryRanking)
    {
        var result = await client.GetPointsRankingsAsync(type: type);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve rankings: {result.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("No rankings found.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var paginationBuilder = new ListPaginationBuilder<PointsRanking>(paginationComponentHandler)
            .WithItems(result.Response.Data)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageData, pageNumber) => GetPointsEmbed(pageData, pageNumber, title, isCountryRanking))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();
        await FollowupAsync(embed: embed, components: components);
    }

    private async Task HandleAverageFinishRanking(string type, string title, bool isCountryRanking)
    {
        var result = await client.GetAverageFinishRankingsAsync(type: type);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve rankings: {result.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("No rankings found.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var paginationBuilder = new ListPaginationBuilder<AverageFinishRanking>(paginationComponentHandler)
            .WithItems(result.Response.Data)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageData, pageNumber) => GetAverageFinishEmbed(pageData, pageNumber, title, isCountryRanking))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();
        await FollowupAsync(embed: embed, components: components);
    }

    private async Task HandleAverageRankRanking(string type, string title, bool isCountryRanking)
    {
        var result = await client.GetAverageRankRankingsAsync(type: type);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve rankings: {result.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("No rankings found.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var paginationBuilder = new ListPaginationBuilder<AverageRankRanking>(paginationComponentHandler)
            .WithItems(result.Response.Data)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageData, pageNumber) => GetAverageRankEmbed(pageData, pageNumber, title, isCountryRanking))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();
        await FollowupAsync(embed: embed, components: components);
    }

    private async Task HandleTotalTimeRanking(string type, string title, bool isCountryRanking)
    {
        var result = await client.GetTotalTimeRankingsAsync(type: type);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve rankings: {result.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("No rankings found.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var paginationBuilder = new ListPaginationBuilder<TotalTimeRanking>(paginationComponentHandler)
            .WithItems(result.Response.Data)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageData, pageNumber) => GetTotalTimeEmbed(pageData, pageNumber, title, isCountryRanking))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();
        await FollowupAsync(embed: embed, components: components);
    }

    private async Task HandleSrPrRanking(string type, string title, bool isCountryRanking)
    {
        var result = await client.GetSrPrRankingsAsync(type: type);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve rankings: {result.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("No rankings found.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var paginationBuilder = new ListPaginationBuilder<SrPrRanking>(paginationComponentHandler)
            .WithItems(result.Response.Data)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pageData, pageNumber) => GetSrPrEmbed(pageData, pageNumber, title, isCountryRanking))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();
        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetPointsEmbed(List<PointsRanking> data, int pageNumber, string title, bool isCountryRanking)
    {
        var placements = new List<string>();
        var names = new List<string>();
        var values = new List<string>();

        foreach (var entry in data)
        {
            placements.Add(RankFormatter.ToMedalFormat(entry.Placement));
            names.Add(BuildNameString(entry.Name, entry.CountryName, isCountryRanking));
            values.Add($"{entry.Points:N0}");
        }

        return BuildEmbed(title, pageNumber, placements, names, values, isCountryRanking, ":star:", "Points");
    }

    private EmbedBuilder GetAverageFinishEmbed(List<AverageFinishRanking> data, int pageNumber, string title, bool isCountryRanking)
    {
        var placements = new List<string>();
        var names = new List<string>();
        var values = new List<string>();

        foreach (var entry in data)
        {
            placements.Add(RankFormatter.ToMedalFormat(entry.Placement));
            names.Add(BuildNameString(entry.Name, entry.CountryName, isCountryRanking));
            values.Add(entry.Average.ToString("F3", CultureInfo.InvariantCulture));
        }

        return BuildEmbed(title, pageNumber, placements, names, values, isCountryRanking, ":chart_with_upwards_trend:", "Avg. Finish");
    }

    private EmbedBuilder GetAverageRankEmbed(List<AverageRankRanking> data, int pageNumber, string title, bool isCountryRanking)
    {
        var placements = new List<string>();
        var names = new List<string>();
        var values = new List<string>();

        foreach (var entry in data)
        {
            placements.Add(RankFormatter.ToMedalFormat(entry.Placement));
            names.Add(BuildNameString(entry.Name, entry.CountryName, isCountryRanking));

            var standardEmote = TierEmoteMapper.MapTierToEmote(entry.StandardId?.ToString() ?? "");
            var avgRankStr = entry.AverageRank.ToString("F3", CultureInfo.InvariantCulture);
            var valueStr = standardEmote != null
                ? $"{standardEmote} {avgRankStr}"
                : avgRankStr;
            values.Add(valueStr);
        }

        return BuildEmbed(title, pageNumber, placements, names, values, isCountryRanking, ":medal:", "Avg. Rank");
    }

    private EmbedBuilder GetTotalTimeEmbed(List<TotalTimeRanking> data, int pageNumber, string title, bool isCountryRanking)
    {
        var placements = new List<string>();
        var names = new List<string>();
        var values = new List<string>();

        foreach (var entry in data)
        {
            placements.Add(RankFormatter.ToMedalFormat(entry.Placement));
            names.Add(BuildNameString(entry.Name, entry.CountryName, isCountryRanking));
            values.Add(entry.TotalTimeFormatted ?? "-");
        }

        return BuildEmbed(title, pageNumber, placements, names, values, isCountryRanking, ":stopwatch:", "Total Time");
    }

    private EmbedBuilder GetSrPrEmbed(List<SrPrRanking> data, int pageNumber, string title, bool isCountryRanking)
    {
        var placements = new List<string>();
        var names = new List<string>();
        var values = new List<string>();

        foreach (var entry in data)
        {
            placements.Add(RankFormatter.ToMedalFormat(entry.Placement));
            names.Add(BuildNameString(entry.Name, entry.CountryName, isCountryRanking));
            values.Add((entry.SrPr * 100).ToString("F3", CultureInfo.InvariantCulture) + "%");
        }

        return BuildEmbed(title, pageNumber, placements, names, values, isCountryRanking, ":checkered_flag:", "SR/PR");
    }

    private string BuildNameString(string? name, string? countryName, bool isCountryRanking)
    {
        var sb = new StringBuilder();

        if (isCountryRanking)
        {
            sb.Append(CountryEmoteMapper.MapCountryToEmote(countryName));
            sb.Append(' ');
            sb.Append(MessageTextFormat.Bold(countryName ?? "Unknown"));
        }
        else
        {
            sb.Append(CountryEmoteMapper.MapCountryToEmote(countryName));
            sb.Append(' ');
            sb.Append(MessageTextFormat.Bold(name ?? "Unknown"));
        }

        return sb.ToString();
    }

    private EmbedBuilder BuildEmbed(string title, int pageNumber, List<string> placements, List<string> names, List<string> values, bool isCountryRanking, string metricEmote, string metricName)
    {
        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}] {title}");

        embed.AddField(new EmbedFieldBuilder()
            .WithName($":trophy: {MessageTextFormat.Bold("Rank")}")
            .WithValue(string.Join("\n", placements))
            .WithIsInline(true));

        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{(isCountryRanking ? ":globe_with_meridians:" : ":bust_in_silhouette:")} {MessageTextFormat.Bold(isCountryRanking ? "Country" : "Player")}")
            .WithValue(string.Join("\n", names))
            .WithIsInline(true));

        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{metricEmote} {MessageTextFormat.Bold(metricName)}")
            .WithValue(string.Join("\n", values))
            .WithIsInline(true));

        return embed;
    }
}
