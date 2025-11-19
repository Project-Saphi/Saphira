namespace Saphira.Discord.Interaction.Autocompletion.ValueProvider;

public class Value(int id, string name)
{
    public readonly int Id = id;
    public readonly string Name = name;
}

public interface IValueProvider
{
    public Task<List<Value>> GetValuesAsync();
}
