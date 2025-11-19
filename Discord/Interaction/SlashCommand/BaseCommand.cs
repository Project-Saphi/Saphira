using Discord.Interactions;
using Saphira.Discord.Interaction.SlashCommand.Metadata;

namespace Saphira.Discord.Interaction.SlashCommand;

public abstract class BaseCommand : InteractionModuleBase<SocketInteractionContext>
{
    public abstract SlashCommandMetadata GetMetadata();    
}
