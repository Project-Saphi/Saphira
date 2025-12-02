using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Core.Logging;
using Saphira.Discord.Entity.Guild.Role;
using Saphira.Discord.Interaction.Foundation.Precondition;
using Saphira.Discord.Interaction.SlashCommand.Metadata;
using Saphira.Discord.Messaging;
using Saphira.Saphi.Api;
using System.Text;

namespace Saphira.Discord.Interaction.SlashCommand;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class VerifyCommand(ISaphiApiClient client, IMessageLogger logger) : BaseCommand
{
    public override SlashCommandMetadata GetMetadata()
    {
        return new SlashCommandMetadata(
            "/verify",
            "Let Saphira guide you through the verification process"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("verify", "Verify yourself")]
    public async Task HandleCommand()
    {
        await DeferAsync();

        var verifiedRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == GuildRole.VerifiedRole);

        if (verifiedRole == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"The {MessageTextFormat.Bold(GuildRole.VerifiedRole)} role does not exist on this server.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (Context.User is not SocketGuildUser guildUser)
        {
            var errorAlert = new ErrorAlertEmbedBuilder("User not found.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        if (guildUser.Roles.Contains(verifiedRole))
        {
            var infoAlert = new InfoAlertEmbedBuilder($"You are already verified.");
            await FollowupAsync(embed: infoAlert.Build());
            return;
        }

        var playerResult = await client.GetPlayersAsync(discord: guildUser.Id.ToString(), forceRefresh: true);

        if (!playerResult.Success || playerResult.Response == null || playerResult.Response.Data.Count == 0)
        {
            await ShowVerificationInstructions(guildUser);
            return;
        }

        await VerifyUser(guildUser, verifiedRole);
    }

    private async Task ShowVerificationInstructions(SocketGuildUser guildUser)
    {
        var instructions = new StringBuilder()
            .AppendLine($":book: {MessageTextFormat.Bold("Verification Instructions")}")
            .AppendLine("")
            .AppendLine("To verify yourself, please enter your Discord ID in your user profile on the website.")
            .AppendLine($"Your Discord ID is {MessageTextFormat.Code(guildUser.Id.ToString())}.")
            .AppendLine("")
            .AppendLine($"After entering your Discord ID in your user profile, run {MessageTextFormat.Code("/verify")} again.");

        var instructionsAlert = new InfoAlertEmbedBuilder(instructions.ToString());
        await FollowupAsync(embed: instructionsAlert.Build());
    }

    private async Task VerifyUser(SocketGuildUser guildUser, SocketRole verifiedRole)
    {
        try
        {
            await guildUser.AddRoleAsync(verifiedRole);

            var successAlert = new SuccessAlertEmbedBuilder($"You are now verified.");
            await FollowupAsync(embed: successAlert.Build());

            logger.Log(LogSeverity.Info, Context.User.Username, $"Verified user {guildUser.Username} ({guildUser.Id})");
        }
        catch (Exception ex)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"Failed to verify you: {ex.Message}");
            await FollowupAsync(embed: errorAlert.Build());
        }
    }
}
