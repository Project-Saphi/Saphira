using Discord.Interactions;
using Saphira.Commands.Metadata;

namespace Saphira.Commands;

public abstract class BaseCommand : InteractionModuleBase<SocketInteractionContext>
{
    public abstract CommandMetadata GetMetadata();    
}
