using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using Saphira.Saphi.Entity;
using Saphira.Util.Game;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class PBsCommand(CachedClient client) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("pbs", "Get personal best times of a player")]
    public async Task HandleCommand(string player)
    {
        var result = await client.GetPlayerPBsAsync(player);

        if (!result.Success || result.Response == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to retrieve personal best times: {result.ErrorMessage ?? "Unknown error"}");
            await RespondAsync(embed: errorAlert.Build());
            return;
        }

        var playerName = result.Response.Data.First().Holder;

        var embed = new EmbedBuilder();

        AddEmbedField(embed, ":motorway:", "Track", GetCustomTracks(result.Response.Data));
        AddEmbedField(embed, ":stopwatch:", "Time", GetTimes(result.Response.Data));
        AddEmbedField(embed, ":trophy:", "Placement", GetPlacements(result.Response.Data));

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
        pbs.Select(p => MessageTextFormat.Bold(p.TrackName)).ToList();

    private List<string> GetTimes(List<PlayerPB> pbs) =>
        pbs.Select(p => ScoreFormatter.AsIngameTime(p.Time)).ToList();

    private List<string> GetPlacements(List<PlayerPB> pbs) =>
        pbs.Select(p => RankFormatter.ToMedalFormat(int.Parse(p.Rank))).ToList();
}
