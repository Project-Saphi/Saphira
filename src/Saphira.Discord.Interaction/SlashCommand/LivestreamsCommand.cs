using Discord;
using Discord.Interactions;
using Saphira.Discord.Guild;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class LivestreamsCommand : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/livestreams",
            "This command can only detect streams from users whose Discord status is set to `Streaming`"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("livestreams", "List all CTR livestreams from server members")]
    public async Task HandleCommand()
    {
        var livestreams = GuildManager.GetCTRStreamActivites(Context.Guild);

        if (livestreams.Count > 0)
        {
            await RespondAsync("People are streaming CTR ...");
            return;
        }
        else
        {
            var infoAlert = new InfoAlertEmbedBuilder("There is currently nobody streaming CTR.");
            await RespondAsync(embed: infoAlert.Build());
            return;
        }
    }
}
