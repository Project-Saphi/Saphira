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
using Saphira.Saphi.Core;
using Saphira.Saphi.Interaction.Autocompletion.ValueProvider;
using System.Text;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class PBsCommand(ISaphiApiClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 15;

    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/pbs TheKoji",
            $"{EntriesPerPage} entries are shown per page"
        );
    }

    [SlashCommand("pbs", "Get personal best times of a player")]
    public async Task HandleCommand(
        [Autocomplete(typeof(GenericAutocompleteHandler<PlayerValueProvider>))] int player
        )
    {
        await DeferAsync();

        var userProfileResult = await client.GetUserProfileAsync(player);

        if (!userProfileResult.Success || userProfileResult.Response == null || userProfileResult.Response.Data.Count == 0)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve player profile: {userProfileResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var userProfile = userProfileResult.Response.Data.First();
        var playerName = userProfile.Name;

        var initialResult = await client.GetPlayerPBsAsync(player, page: 1, perPage: EntriesPerPage);

        if (!initialResult.Success || initialResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve personal best times: {initialResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (initialResult.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("This player doesn't have any PBs set yet.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        var totalItems = initialResult.Response.Meta.Total;

        var paginationBuilder = new CallbackPaginationBuilder<PlayerPB>(paginationComponentHandler)
            .WithPageSize(EntriesPerPage)
            .WithTotalItems(totalItems)
            .WithFetchCallback(async (page, perPage) =>
            {
                var result = await client.GetPlayerPBsAsync(player, page: page, perPage: perPage);
                return result.Response?.Data ?? [];
            })
            .WithRenderPageCallback((pagePBs, pageNumber, totalPages) => GetEmbedForPage(playerName, pagePBs, pageNumber, totalPages))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = await paginationBuilder.BuildAsync();

        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(string playerName, List<PlayerPB> pagePBs, int pageNumber, int totalPages)
    {
        if (pagePBs.Count == 0)
        {
            return new EmbedBuilder().WithDescription("No entries found.");
        }

        var pbData = GetPBData(pagePBs);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}/{totalPages}] {playerName}'s personal best times");

        AddEmbedField(embed, ":motorway:", "Track", pbData["tracks"]);
        AddEmbedField(embed, ":stadium:", "Category", pbData["categories"]);
        AddEmbedField(embed, ":stopwatch:", "Time", pbData["times"]);

        return embed;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private Dictionary<string, List<string>> GetPBData(List<PlayerPB> playerPBs)
    {
        var dict = new Dictionary<string, List<string>>()
        {
            { "tracks", [] },
            { "categories", [] },
            { "times", [] }
        };

        foreach (var playerPB in playerPBs)
        {
            dict["tracks"].Add(BuildTrackString(playerPB));
            dict["categories"].Add(BuildCategoryString(playerPB));
            dict["times"].Add(BuildTimeString(playerPB));
        }

        return dict;
    }

    private string BuildTrackString(PlayerPB playerPB)
    {
        return new StringBuilder()
            .Append(RankFormatter.ToMedalFormat(playerPB.Rank))
            .Append(' ')
            .Append(MessageTextFormat.Bold(playerPB.TrackName))
            .ToString();
    }

    private string BuildCategoryString(PlayerPB playerPB)
    {
        var standardEmote = playerPB.StandardId.HasValue
            ? TierEmoteMapper.MapTierToEmote(playerPB.StandardId.Value.ToString())
            : null;

        var categoryString = new StringBuilder();

        if (standardEmote != null)
        {
            categoryString
                .Append(standardEmote)
                .Append(' ');
        }

        return categoryString
            .Append(MessageTextFormat.Bold(playerPB.CategoryName))
            .ToString();
    }

    private string BuildTimeString(PlayerPB playerPB)
    {
        return new StringBuilder()
            .Append(CharacterEmoteMapper.MapCharacterToEmote(playerPB.CharacterName))
            .Append(' ')
            .Append(playerPB.TimeFormatted)
            .ToString();
    }
}
