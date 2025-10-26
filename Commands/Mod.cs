
using Discord.Interactions;
using Saphira.Discord;

namespace Saphira.Commands
{
    public class Mod : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("post", "Send a message as Saphira")]
        public async Task PostCommand()
        {
            if (!GuildMember.IsTeamMember(Context.User))
            {
                var errorAlert = new ErrorAlertEmbedBuilder("You need to be a team member to use this command.");
                await RespondAsync(embed: errorAlert.Build());
                return;
            }
        }
    }
}
