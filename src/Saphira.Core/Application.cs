using System.Reflection;

namespace Saphira.Core;

public static class Application
{
    public static DateTime StartTime { get; set; }

    public static List<Assembly> LoadAssemblies()
    {
        var assemblies = new List<Assembly>();

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith("Saphira"))
            .ToList();

        assemblies.AddRange(loadedAssemblies);

        var assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var assemblyFiles = Directory.GetFiles(assemblyDirectory, "Saphira.*.dll");

        foreach (var assemblyFile in assemblyFiles)
        {
            var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);

            if (loadedAssemblies.Any(a => a.FullName == assemblyName.FullName))
            {
                continue;
            }

            try
            {
                var assembly = Assembly.Load(assemblyName);
                assemblies.Add(assembly);
            }
            catch
            {
            }
        }

        return assemblies;
    }
}
