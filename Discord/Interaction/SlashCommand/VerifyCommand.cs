using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Discord.Interaction.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Util.Logging;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
[RequireTeamMemberRole]
public class VerifyCommand(IMessageLogger logger) : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/verify @RedHot",
            "The user must be a member of the server"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("verify", "Verify a user")]
    public async Task HandleCommand(SocketGuildUser user)
    {
        await DeferAsync();

        var verifiedRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == Guild.GuildRole.VerifiedRole);

        if (verifiedRole == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"The {MessageTextFormat.Bold(Guild.GuildRole.VerifiedRole)} role does not exist on this server.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (user.Roles.Contains(verifiedRole))
        {
            var infoAlert = new InfoAlertEmbedBuilder($"{user.Mention} is already verified.");
            await FollowupAsync(embed: infoAlert.Build());
            return;
        }

        try
        {
            await user.AddRoleAsync(verifiedRole);

            var successAlert = new SuccessAlertEmbedBuilder($"Successfully verified {user.Mention}.");
            await FollowupAsync(embed: successAlert.Build());

            logger.Log(LogSeverity.Info, Context.User.Username, $"Verified user {user.Username} ({user.Id})");
        }
        catch (Exception ex)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to verify user: {ex.Message}");
            await FollowupAsync(embed: errorAlert.Build());
        }
    }
}
