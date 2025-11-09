namespace Saphira.Commands.Metadata;

public class CommandMetadata(string? description = null, string? example = null, string? notes = null)
{
    public readonly string? Description = description;
    public readonly string? Example = example;
    public readonly string? Notes = notes;
}
