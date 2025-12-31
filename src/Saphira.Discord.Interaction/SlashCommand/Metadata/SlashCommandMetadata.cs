namespace Saphira.Discord.Interaction.SlashCommand.Metadata;

public class SlashCommandMetadata(string? example = null, string? notes = null)
{
    public readonly string? Example = example;
    public readonly string? Notes = notes;
}
