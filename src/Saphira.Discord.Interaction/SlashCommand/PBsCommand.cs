using Discord;
using Discord.Interactions;
using Saphira.Discord.Interaction.Autocompletion;
using Saphira.Discord.Interaction.Autocompletion.ValueProvider;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Discord.Messaging.Pagination;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.EmoteMapper;
using Saphira.Util.Game;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireCooldownExpired(15)]
[RequireTextChannel]
[RequireCommandAllowedChannel]
public class PBsCommand(CachedClient client, PaginationComponentHandler paginationComponentHandler) : BaseCommand
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
        [Autocomplete(typeof(GenericAutocompleteHandler<PlayerValueProvider>))] string player
        )
    {
        var result = await client.GetPlayerPBsAsync(player);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve personal best times: {result.ErrorMessage ?? "Unknown error"}");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        if (result.Response.Data.Count == 0)
        {
            var warningAlert = new WarningAlertEmbedBuilder("This player doesn't have any PBs set yet.");
            await RespondAsync(embed: warningAlert.Build());
            return;
        }

        var playerPBs = result.Response.Data;
        var playerName = playerPBs.First().Holder;

        var paginationBuilder = new PaginationBuilder<PlayerPB>(paginationComponentHandler)
            .WithItems(playerPBs)
            .WithPageSize(EntriesPerPage)
            .WithRenderPageCallback((pagePBs, pageNumber) => GetEmbedForPage(playerName, pagePBs, pageNumber));

        var (embed, components) = paginationBuilder.Build();

        await RespondAsync(embed: embed, components: components);
    }

    private EmbedBuilder GetEmbedForPage(string playerName, List<PlayerPB> pagePBs, int pageNumber)
    {
        var embed = new EmbedBuilder()
            .WithAuthor($"[Page {pageNumber}] {playerName}'s personal best times");

        AddEmbedField(embed, ":motorway:", "Track", GetCustomTracks(pagePBs));
        AddEmbedField(embed, ":stadium:", "Category", GetCategories(pagePBs));
        AddEmbedField(embed, ":stopwatch:", "Time", GetTimes(pagePBs));

        return embed;
    }

    private void AddEmbedField(EmbedBuilder embed, string emote, string title, List<string> content)
    {
        embed.AddField(new EmbedFieldBuilder()
            .WithName($"{emote} {MessageTextFormat.Bold(title)}")
            .WithValue(string.Join("\n", content))
            .WithIsInline(true));
    }

    private List<string> GetCustomTracks(List<PlayerPB> pbs) =>
        [.. pbs.Select(p => MessageTextFormat.Bold(p.TrackName))];

    private List<string> GetCategories(List<PlayerPB> pbs) =>
    [.. pbs.Select(p => MessageTextFormat.Bold(p.CategoryName))];

    private List<string> GetTimes(List<PlayerPB> pbs) =>
        [.. pbs.Select(p => $"{RankFormatter.ToMedalFormat(int.Parse(p.Rank))} - {ScoreFormatter.AsHumanTime(p.Time)} {CharacterEmoteMapper.MapCharacterToEmote(p.CharacterName)}")];
}
