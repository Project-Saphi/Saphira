using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Precondition;
using Saphira.Discord.Messaging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
[RequireTeamMemberRole]
public class VerifyCommand : InteractionModuleBase<SocketInteractionContext>
{
    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("verify", "Verify a user")]
    public async Task HandleCommand(SocketGuildUser user)
    {
        await DeferAsync();

        var verifiedRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == Discord.Guild.GuildRole.VerifiedRole);

        if (verifiedRole == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"The {MessageTextFormat.Bold(Discord.Guild.GuildRole.VerifiedRole)} role does not exist on this server.");
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
        }
        catch (Exception ex)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to verify user: {ex.Message}");
            await FollowupAsync(embed: errorAlert.Build());
        }
    }
}
