using Discord;
using Discord.Interactions;
using Saphira.Commands.Precondition;
using Saphira.Discord.Guild;
using Saphira.Discord.Messaging;

namespace Saphira.Commands
{
    [RequireTextChannel]
    [RequireCommandAllowedChannel]
    public class LivestreamsCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly GuildManager _guildManager;

        public LivestreamsCommand(GuildManager guildManager)
        {
            _guildManager = guildManager;
        }

        [CommandContextType(InteractionContextType.Guild)]
        [SlashCommand("livestreams", "List all CTR livestreams from server members")]
        public async Task HandleCommand()
        {
            var livestreams = _guildManager.GetCTRStreamActivites(Context.Guild);

            if (livestreams.Count > 0)
            {
                await RespondAsync("People are streaming CTR ...");
                return;
            }
            else
            {
                var warningAlert = new WarningAlertEmbedBuilder("There is currently nobody streaming CTR.");
                await RespondAsync(embed: warningAlert.Build());
                return;
            }
        }
    }
}
