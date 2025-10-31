using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Saphira.Commands.Autocompletion;
using Saphira.Commands.Precondition;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    public class ToggleCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly GuildRoleManager _guildRoleManager;

        public ToggleCommand(GuildRoleManager guildRoleManager)
        {
            _guildRoleManager = guildRoleManager;
        }

        [CommandContextType(InteractionContextType.Guild)]
        [SlashCommand("toggle", "Toggle one of your roles off or on")]
        public async Task HandleCommand(
            [Autocomplete(typeof(ToggleableRoleAutocompleteHandler))] int role
            )
        {
            await DeferAsync();

            var toggleableRoles = _guildRoleManager.GetToggleableRoles();
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

            if (guildUser.Roles.Contains(guildRole))
            {
                await guildUser.RemoveRoleAsync(guildRole);

                var successAlert = new SuccessAlertEmbedBuilder($"Removed role {MessageTextFormat.Bold(guildRole.Name)} from you.");
                await FollowupAsync(embed: successAlert.Build());
            }
            else
            {
                await guildUser.AddRoleAsync(guildRole);

                var successAlert = new SuccessAlertEmbedBuilder($"Added role {MessageTextFormat.Bold(guildRole.Name)} to you.");
                await FollowupAsync(embed: successAlert.Build());
            }
        }
    }
}
