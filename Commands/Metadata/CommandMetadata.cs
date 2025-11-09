namespace Saphira.Commands.Metadata;

public class CommandMetadata(string? example = null, string? notes = null)
{
    public readonly string? Example = example;
    public readonly string? Notes = notes;
}
