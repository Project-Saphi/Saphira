using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;

namespace Saphira.Commands
{
    public class ProfileCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [CommandContextType(InteractionContextType.Guild)]
        [RequireTextChannel]
        [SlashCommand("profile", "See your user profile")]
        public async Task HandleCommand()
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(Context.User.GlobalName);

            await RespondAsync(embed: embed.Build());
        }
    }
}
