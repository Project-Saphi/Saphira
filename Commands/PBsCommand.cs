using Discord;
using Discord.Interactions;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Metadata;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;
using Saphira.Util.Mapper;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class PBsCommand(CachedClient client) : BaseCommand
{
    public override CommandMetadata GetMetadata()
    {
        return new CommandMetadata("/pbs TheKoji");
    }

    [SlashCommand("pbs", "Get personal best times of a player")]
    public async Task HandleCommand(
        [Autocomplete(typeof(PlayerAutocompleteHandler))] string player
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

        var playerName = result.Response.Data.First().Holder;

        var embed = new EmbedBuilder()
            .WithAuthor($"Personal best times of {playerName}");

        AddEmbedField(embed, ":motorway:", "Track", GetCustomTracks(result.Response.Data));
        AddEmbedField(embed, ":stadium:", "Category", GetCategories(result.Response.Data));
        AddEmbedField(embed, ":stopwatch:", "Time", GetTimes(result.Response.Data));

        await RespondAsync(embed: embed.Build());
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
        [.. pbs.Select(p => $"{RankFormatter.ToMedalFormat(int.Parse(p.Rank))} - {ScoreFormatter.AsIngameTime(p.Time)} {CharacterEmoteMapper.MapCharacterToEmote(p.CharacterName)}")];
}
