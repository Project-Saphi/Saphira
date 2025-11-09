using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Metadata;
using Saphira.Commands.Precondition;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;

namespace Saphira.Commands;

[RequireTextChannel]
[RequireCommandAllowedChannel]
public class ToggleCommand(GuildRoleManager guildRoleManager) : BaseCommand
{
    public override CommandMetadata GetMetadata()
    {
        return new CommandMetadata(
            "Toggle one of your roles off or on",
            "/toggle Server Updates",
            "Limited to roles which can be toggled on or off (not any arbitrary roles)"
        );
    }

    [CommandContextType(InteractionContextType.Guild)]
    [SlashCommand("toggle", "Toggle one of your roles off or on")]
    public async Task HandleCommand(
        [Autocomplete(typeof(ToggleableRoleAutocompleteHandler))] int role
        )
    {
        await DeferAsync();

        var toggleableRoles = guildRoleManager.GetToggleableRoles();
        var toggledRole = toggleableRoles[role];

        if (Context.User is not SocketGuildUser guildUser)
        {
            var errorAlert = new ErrorAlertEmbedBuilder("Could not fetch guild user information.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var guildRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == toggledRole);

        if (guildRole == null)
        {
            var errorAlert = new ErrorAlertEmbedBuilder($"The role {MessageTextFormat.Bold(toggledRole)} does not exist.");
            await FollowupAsync(embed: errorAlert.Build());
            return;
        }

        var hasRole = guildUser.Roles.Contains(guildRole);
        await (hasRole
            ? guildUser.RemoveRoleAsync(guildRole)
            : guildUser.AddRoleAsync(guildRole));

        var action = hasRole ? "Removed" : "Added";
        var preposition = hasRole ? "from" : "to";
        var message = $"{action} role {MessageTextFormat.Bold(guildRole.Name)} {preposition} you.";

        var successAlert = new SuccessAlertEmbedBuilder(message);
        await FollowupAsync(embed: successAlert.Build());
    }
}
