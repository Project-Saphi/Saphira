namespace Saphira.Commands.Autocompletion.ValueProvider
{
    public class Value
    {
        public readonly int Id;
        public readonly string Name;

        public Value(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public interface IValueProvider
    {
        public Task<List<Value>> GetValuesAsync();
    }
}
