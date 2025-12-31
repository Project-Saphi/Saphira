using Discord;

namespace Saphira.Discord.Core.Interaction;

public class InteractionResponder
{
    public static async Task RespondAsync(IDiscordInteraction interaction, string message)
    {
        if (interaction.HasResponded)
        {
            await interaction.FollowupAsync(message, ephemeral: true);
        }
        else
        {
            await interaction.RespondAsync(message, ephemeral: true);
        }
    }
}
