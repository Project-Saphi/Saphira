using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Foundation.Autocompletion;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.EmoteMapper;
using Saphira.Discord.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Saphi.Game;
using Saphira.Saphi.Interaction;
using System.Text;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class PBsCommand(ISaphiApiClient client, StandardCalculator standardCalculator, PaginationComponentHandler paginationComponentHandler) : BaseCommand
{
    private readonly int EntriesPerPage = 20;

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

        var playerPBsResult = await client.GetPlayerPBsAsync(player);

        if (!playerPBsResult.Success || playerPBsResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve personal best times: {playerPBsResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (playerPBsResult.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("This player doesn't have any PBs set yet.");
            await FollowupAsync(embed: warningAlert.Build());
            return;
        }

        // In theory the PlayerPB object is disconnected from the custom track itself
        // but we need the standards, so ... eh
        var customTrackResult = await client.GetCustomTracksAsync();

        if (!customTrackResult.Success || customTrackResult.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve personal best times: {customTrackResult.ErrorMessage ?? "Unknown error"}");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var playerPBs = playerPBsResult.Response.Data;
        var customTracks = customTrackResult.Response.Data;

        var playerName = playerPBs.First().Holder;

        var paginationBuilder = new PaginationBuilder<PlayerPB>(paginationComponentHandler)
            .WithItems(playerPBs)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pagePBs, pageNumber) => GetEmbedForPage(customTracks, playerName, pagePBs, pageNumber))
            .WithFilter((component) => Task.FromResult(new PaginationFilterResult(component.User.Id == Context.User.Id)));

        var (embed, components) = paginationBuilder.Build();

        await FollowupAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(List<CustomTrack> customTracks, string playerName, List<PlayerPB> pagePBs, int pageNumber)
    {
        var pbData = GetPBData(customTracks, pagePBs);

        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}] {playerName}'s personal best times");

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

    private Dictionary<string, List<string>> GetPBData(List<CustomTrack> customTracks, List<PlayerPB> playerPBs)
    {
        var dict = new Dictionary<string, List<string>>()
        {
            { "tracks", [] },
            { "categories", [] },
            { "times", [] }
        };

        foreach (var playerPB in playerPBs)
        {
            var customTrack = customTracks.FirstOrDefault(x => x.Id == playerPB.TrackId);

            if (customTrack == null)
            {
                // not sure what to do in that case, probably just skip the track
                continue;
            }

            dict["tracks"].Add(BuildTrackString(playerPB));
            dict["categories"].Add(BuildCategoryString(customTrack, playerPB));
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

    private string BuildCategoryString(CustomTrack customTrack, PlayerPB playerPB)
    {
        var standard = standardCalculator.CalculateStandard(customTrack, playerPB.CategoryId, playerPB.Time);
        var standardEmote = standard != null ? TierEmoteMapper.MapTierToEmote(standard.TierId.ToString()) : null;

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
            .Append(ScoreFormatter.AsHumanTime(playerPB.Time))
            .ToString();
    }
}
