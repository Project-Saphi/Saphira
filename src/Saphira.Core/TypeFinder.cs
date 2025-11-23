using System.Reflection;

namespace Saphira.Core;

public class TypeFinder(Assembly assembly)
{
    private readonly Assembly _assembly = assembly;
    private Type? _interfaceType = null;
    private Type? _attributeType = null;
    private bool _optionalAttribute = false;

    public TypeFinder ByInterface(Type interfaceType)
    {
        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException($"{interfaceType} is not an interface.");
        }

        _interfaceType = interfaceType;
        return this;
    }

    public TypeFinder ByAttribute(Type attributeType, bool requireAttribute = true)
    {
        if (!attributeType.IsAssignableTo(typeof(System.Attribute)))
        {
            throw new ArgumentException($"{attributeType} is not an attribute.");
        }

        _attributeType = attributeType;
        _optionalAttribute = !requireAttribute;
        return this;
    }

    public IEnumerable<Type> Find()
    {
        IEnumerable<Type> types = _assembly.GetTypes();

        types = types.Where(t => t is { IsInterface: false, IsAbstract: false });

        if (_interfaceType != null)
        {
            types = types.Where(_interfaceType.IsAssignableFrom);
        }

        if (_attributeType != null)
        {
            types = types.Where(t => _optionalAttribute || t.GetCustomAttribute(_attributeType) != null);
        }

        return types;
    }
}
