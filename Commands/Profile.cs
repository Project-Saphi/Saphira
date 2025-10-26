using Discord;
using Discord.Interactions;

namespace Saphira.Commands
{
    public class Profile : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("profile", "See your user profile")]
        public async Task ProfileCommand()
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(Context.User.GlobalName);

            await RespondAsync(embed: embed.Build());
        }
    }
}
