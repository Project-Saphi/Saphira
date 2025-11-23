using Discord;
using Discord.Interactions;

namespace Saphira.Discord.Interaction.Foundation.TypeConverter;

public class EmoteTypeConverter : TypeConverter<IEmote>
{
    public override ApplicationCommandOptionType GetDiscordType()
    {
        return ApplicationCommandOptionType.String;
    }

    public override Task<TypeConverterResult> ReadAsync(
        IInteractionContext context, 
        IApplicationCommandInteractionDataOption option, 
        IServiceProvider services
        )
    {
        var input = option.Value.ToString();

        if (string.IsNullOrWhiteSpace(input))
        {
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, "Emote cannot be empty."));
        }

        if (Emote.TryParse(input, out var customEmote))
        {
            return Task.FromResult(TypeConverterResult.FromSuccess(customEmote));
        }

        try
        {
            var emoji = new Emoji(input);
            return Task.FromResult(TypeConverterResult.FromSuccess(emoji));
        }
        catch
        {
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, "Invalid emote format."));
        }
    }
}
